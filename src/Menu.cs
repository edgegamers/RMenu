using System.Text;
using RMenu.Enums;
using RMenu.Helpers;
using RMenu.Hooks;
using RMenu.Listeners;
using RMenu.Models;

namespace RMenu;

public static partial class Menu {
  public const int MAX_PLAYERS = 64;
  internal const int MENU_HEIGHT = 140;
  internal const int MENU_LENGTH = 300;

  private static readonly StringBuilder MENU_BUILDER = new(8192);
  private static readonly MenuData?[] MENU_DATA = new MenuData[MAX_PLAYERS];

  static Menu() {
    OnSayListener.Register();
    OnTickListener.Register();

    RunCommandHook.Register();
    SpecModeHook.Register();

    Thread menuThread = new(Menu.menuThread) {
      IsBackground = true, Priority = ThreadPriority.Lowest
    };

    menuThread.Start();
  }

  public static event EventHandler<MenuEvent>? OnPrintMenu;

  internal static string RaiseOnPrintMenu(MenuBase menu, string html) {
    MenuEvent menuEvent = new(menu, html);
    OnPrintMenu?.Invoke(null, menuEvent);
    return menuEvent.Html;
  }

  private static void menuThread() {
    while (true) {
      Rainbow.Update();
      processMenus();

      Thread.Sleep(100);
    }
  }

  private static void processMenus() {
    for (var i = 0; i < MAX_PLAYERS; i++) {
      if (MENU_DATA[i] is not { } menuData) continue;

      if (menuData.Menus.Count == 0 || menuData.Menus[0].Count == 0) {
        MENU_DATA[i] = null;
        continue;
      }

      var menu = menuData.Menus[0][^1];
      menuData.Current = (menu, renderMenu(menu));
    }
  }

  private static string renderMenu(MenuBase menu) {
    _ = MENU_BUILDER.Clear();
    _ = MENU_BUILDER.Append('\u00A0');

    if (menu.Header is { } header) renderHeader(MENU_BUILDER, menu, header);

    renderBody(MENU_BUILDER, menu);

    if (menu.Footer is { } footer) renderFooter(MENU_BUILDER, menu, footer);

    return MENU_BUILDER.ToString();
  }

  private static void renderHeader(StringBuilder stringBuilder, MenuBase menu,
    MenuValue header) {
    _ = stringBuilder.Append(menu.Options.HeaderSizeHtml);
    header.render(stringBuilder);

    if (menu.Options.DisplayItemsInHeader) {
      var isSubMenu = MENU_DATA[menu.Player.Slot]?.Menus[0].Count > 1;

      if (isSubMenu || menu.SelectedItem is not null)
        _ = stringBuilder.Append($"</font>{menu.Options.FooterSizeHtml}");

      if (isSubMenu) _ = stringBuilder.Append(" ⇦");

      if (menu.SelectedItem is not null)
        _ = stringBuilder.Append(
          $" {menu.SelectedItem.Index + 1}/{menu.Items.Count}");
    }

    _ = stringBuilder.Append("<br>");
  }

  private static void renderFooter(StringBuilder stringBuilder, MenuBase menu,
    MenuValue footer) {
    _ = stringBuilder.Append($"</font>{menu.Options.FooterSizeHtml}");
    footer.render(stringBuilder);
  }

  private static void renderBody(StringBuilder stringBuilder, MenuBase menu) {
    int start, end;

    if (menu.Options.Paginate) {
      var pageSize = menu.Options.AvailableItems;
      start = menu.CurrentPage * pageSize;
      end   = Math.Min(menu.Items.Count, start + pageSize);
    } else {
      start = 0;
      end   = menu.Items.Count;
      var selectedIndex = menu.SelectedItem?.Index ?? 0;

      if (menu.Items.Count > menu.Options.AvailableItems) {
        var half = menu.Options.AvailableItems / 2;
        start = Math.Max(0, selectedIndex - half);
        end   = Math.Min(menu.Items.Count, start + menu.Options.AvailableItems);

        if (end - start < menu.Options.AvailableItems)
          start = Math.Max(0, end - menu.Options.AvailableItems);
      }
    }

    if (menu.Items.Count != 0)
      _ = stringBuilder.Append($"</font>{menu.Options.ItemSizeHtml}");

    for (var i = start; i < end; i++) {
      renderItem(stringBuilder, menu, menu.Items[i]);

      if (i < end - 1 || menu.Options.Paginate || menu.Footer is not null)
        _ = stringBuilder.Append("<br>");
    }

    if (menu.Options.Paginate) renderPageBar(stringBuilder, menu);
  }

  private static void
    renderPageBar(StringBuilder stringBuilder, MenuBase menu) {
    var current   = menu.CurrentPage;
    var pageCount = menu.PageCount;

    if (pageCount <= 1) {
      // Still reserve the line so layout is stable
      _ = stringBuilder.Append('\u00A0');
      if (menu.Footer is not null) _ = stringBuilder.Append("<br>");
      return;
    }

    _ = stringBuilder.Append("</font>");
    _ = stringBuilder.Append(menu.Options.FooterSizeHtml);
    _ = stringBuilder.Append("Page: ");

    // Show at most ~7 page tokens; use "..." for overflow
    const int window = 3; // pages shown either side of current

    for (var p = 0; p < pageCount; p++) {
      var distFromCurrent = Math.Abs(p - current);
      var isFirst         = p == 0;
      var isLast          = p == pageCount - 1;
      var inWindow        = distFromCurrent <= window;

      if (!isFirst && !isLast && !inWindow) {
        // Emit ellipsis once when transitioning into a skipped range
        var prevDist                            = Math.Abs((p - 1) - current);
        if (prevDist <= window || p - 1 == 0) _ = stringBuilder.Append("... ");
        continue;
      }

      _ = p == current ?
        stringBuilder.Append($"[{p + 1}] ") :
        stringBuilder.Append($"{p + 1} ");
    }

    if (menu.Footer is not null) _ = stringBuilder.Append("<br>");
  }

  private static void renderItem(StringBuilder stringBuilder, MenuBase menu,
    MenuItem menuItem) {
    if (menuItem.Type is MenuItemType.SPACER) {
      _ = stringBuilder.Append('\u00A0');
      return;
    }

    var isSelected = menuItem == menu.SelectedItem?.Item;

    var isSingleButton = menuItem.Type is MenuItemType.BUTTON
      && menuItem.Values is not { Count: > 0 };

    var headLength =
      menuItem.Head?.calculateLength(isSelected ? menu.Options.Highlight : null)
      ?? 0;
    var tailLength =
      menuItem.Tail?.calculateLength(isSelected ? menu.Options.Highlight : null)
      ?? 0;

    switch (menuItem.Options.Trim) {
      case MenuTrim.HEAD when menuItem.Head is not null:
        trimValue(menuItem.Head, menu.Options.AvailableChars - tailLength);
        break;
      case MenuTrim.TAIL when menuItem.Tail is not null:
        trimValue(menuItem.Tail, menu.Options.AvailableChars - headLength);
        break;
    }

    if (isSelected) menu.Options.Cursor[0].render(stringBuilder);

    if (isSingleButton) renderSelector(stringBuilder, menu, menuItem, 0);

    menuItem.Head?.render(stringBuilder,
      isSelected ? menu.Options.Highlight : null);
    formatType(stringBuilder, menu, menuItem);
    menuItem.Tail?.render(stringBuilder,
      isSelected ? menu.Options.Highlight : null);

    if (isSingleButton) renderSelector(stringBuilder, menu, menuItem, 1);

    if (isSelected) menu.Options.Cursor[1].render(stringBuilder);
  }

  private static void renderSelector(StringBuilder stringBuilder, MenuBase menu,
    MenuItem menuItem, int index) {
    if (menuItem.Type is MenuItemType.BUTTON
      && menuItem != menu.SelectedItem?.Item)
      return;

    menu.Options.Selector[index].render(stringBuilder);
  }

  private static void formatType(StringBuilder stringBuilder, MenuBase menu,
    MenuItem menuItem) {
    switch (menuItem.Type) {
      case MenuItemType.INPUT:
        formatInput(stringBuilder, menu, menuItem);
        break;

      case MenuItemType.BUTTON or MenuItemType.CHOICE:
        formatValues(stringBuilder, menu, menuItem);
        break;
    }
  }

  private static void formatInput(StringBuilder stringBuilder, MenuBase menu,
    MenuItem menuItem) {
    var remainingChars = menu.Options.AvailableChars;
    var isSelected     = menuItem == menu.SelectedItem?.Item;

    if (menuItem.Head is { } head)
      remainingChars -= head.calculateLength(menu.Options.Highlight);

    if (menuItem.Tail is { } tail)
      remainingChars -= tail.calculateLength(menu.Options.Highlight);

    if (menu.Text && isSelected)
      renderSelector(stringBuilder, menu, menuItem, 0);

    if (menuItem.Data is string input) {
      var trimmed = trimString(input, remainingChars);
      _ = stringBuilder.Append(trimmed);
    } else { menu.Options.Input.render(stringBuilder); }

    if (menu.Text && isSelected)
      renderSelector(stringBuilder, menu, menuItem, 1);
  }

  private static void formatValues(StringBuilder stringBuilder, MenuBase menu,
    MenuItem menuItem) {
    if (menuItem.Values is not { Count: > 0 }) return;

    var currentIndex = menuItem.SelectedValue?.Index ?? 0;
    var previousIndex = currentIndex == 0 ?
      menuItem.Values.Count - 1 :
      currentIndex - 1;
    var nextIndex = currentIndex == menuItem.Values.Count - 1 ?
      0 :
      currentIndex + 1;

    var selectedLength = menuItem.Values[currentIndex]
     .calculateLength(menu.Options.Highlight);
    var remainingChars = menu.Options.AvailableChars - selectedLength;

    if (menuItem.Head is { } head)
      remainingChars -= head.calculateLength(menu.Options.Highlight);

    if (menuItem.Tail is { } tail)
      remainingChars -= tail.calculateLength(menu.Options.Highlight);

    var renderItems = 1;

    if (menuItem.Options.Pinwheel || menuItem.Values.Count > 2) renderItems = 2;

    var splitChars = remainingChars / renderItems < 1 ?
      1 :
      remainingChars / renderItems;

    trimValue(menuItem.Values[previousIndex], splitChars);
    trimValue(menuItem.Values[nextIndex], splitChars);

    trimValue(menuItem.Values[currentIndex],
      remainingChars + selectedLength - splitChars * renderItems);

    if (menuItem.Options.Pinwheel || currentIndex > 0
      && currentIndex < menuItem.Values.Count - 1) {
      menuItem.Values[previousIndex].render(stringBuilder);
      _ = stringBuilder.Append(' ');
      formatSelected(stringBuilder, menu, menuItem,
        menuItem.Values[currentIndex]);
      _ = stringBuilder.Append(' ');
      menuItem.Values[nextIndex].render(stringBuilder);
    } else if (currentIndex == 0) {
      formatSelected(stringBuilder, menu, menuItem,
        menuItem.Values[currentIndex]);

      for (var i = 0; i < 2 && i < menuItem.Values.Count - 1; i++) {
        trimValue(menuItem.Values[i + 1], splitChars);

        _ = stringBuilder.Append(' ');
        menuItem.Values[i + 1].render(stringBuilder);
      }
    } else {
      for (var i = 2; i > 0; i--)
        if (currentIndex - i >= 0) {
          trimValue(menuItem.Values[currentIndex - i], splitChars);

          menuItem.Values[currentIndex - i].render(stringBuilder);
          _ = stringBuilder.Append(' ');
        }

      formatSelected(stringBuilder, menu, menuItem,
        menuItem.Values[currentIndex]);
    }
  }

  private static void formatSelected(StringBuilder stringBuilder, MenuBase menu,
    MenuItem menuItem, MenuValue menuValue) {
    renderSelector(stringBuilder, menu, menuItem, 0);
    menuValue.render(stringBuilder, menu.Options.Highlight);
    renderSelector(stringBuilder, menu, menuItem, 1);
  }

  private static void trimValue(MenuValue menuValue, int remainingChars) {
    remainingChars = Math.Max(1, remainingChars);

    foreach (var menuObject in menuValue.Objects) {
      menuObject.Display = trimString(menuObject.Text, remainingChars);

      remainingChars -= menuObject.Display.Length;
    }
  }

  private static string trimString(string input, int remainingChars) {
    if (remainingChars < 1) return string.Empty;

    if (input.Length <= remainingChars) return input;

    return input[..(remainingChars - 1)] + '.';
  }
}