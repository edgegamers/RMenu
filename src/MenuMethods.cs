using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using RMenu.Enums;
using RMenu.Extensions;
using RMenu.Models;

namespace RMenu;

public static partial class Menu {
  private static readonly MenuButton[] MENU_BUTTONS =
    Enum.GetValues<MenuButton>();

  public static void Display(CCSPlayerController player, MenuBase menu,
    bool subMenu = false, Action<MenuBase, MenuAction>? callback = null) {
    menu.Player   = player;
    menu.Callback = callback;

    for (var i = 0; i < menu.Items.Count; i++) {
      var menuItem = menu.Items[i];

      if (menu.SelectedItem is null && isSelectable(menuItem))
        menu.SelectedItem = new MenuSelectedItem(i, menuItem);

      menuItem.Callback?.Invoke(menu, menuItem, MenuAction.START);
    }

    menu.Callback?.Invoke(menu, MenuAction.START);

    if (getData(player.Slot) is not { } menuData) {
      menuData               = new MenuData(player);
      MENU_DATA[player.Slot] = menuData;
    }

    if (subMenu && menuData.Menus.Count > 0 && menuData.Menus[0].Count > 0) {
      MenuOptions options = new(menuData.Menus[0][^1].Options);
      options.merge(menu.Options);

      menu.Options = options;
      menuData.Menus[0].Add(menu);
    } else {
      List<MenuBase> menuStack = [menu];

      var isInsert = !subMenu && Get(player) is { } parent
        && (menu.Options.Priority >= parent.Options.Priority
          || !parent.Options.Exitable);

      if (isInsert)
        menuData.Menus.Insert(0, menuStack);
      else
        menuData.Menus.Add(menuStack);
    }
    
    if (menu.Options.BlockMovement && player.IsAlive()) 
      player.Freeze();

    menuData.Update();
  }

  public static MenuBase? Get(CCSPlayerController player) {
    if (getData(player.Slot) is not { } menuData) return null;

    if (menuData.Menus.Count == 0 || menuData.Menus[0].Count == 0) return null;

    return menuData.Menus[0][^1];
  }

  public static bool Close(CCSPlayerController player) {
    if (getData(player.Slot) is not { } menuData) return false;

    if (menuData.Menus.Count == 0 || menuData.Menus[0].Count < 2) return false;

    menuData.Menus[0].RemoveAt(menuData.Menus[0].Count - 1);
    
    ensureFreezeState(player, menuData);
    
    menuData.Update();
    return true;
  }

  public static void Clear(CCSPlayerController player, bool force = false) {
    if (getData(player.Slot) is not { } menuData) return;

    for (var i = menuData.Menus.Count; i > 0; i--) {
      var menuStack = menuData.Menus[i - 1];

      if (menuStack.Count != 0 && !menuStack[^1].Options.Exitable && !force)
        continue;

      menuData.Menus.RemoveAt(i - 1);
    }
    
    ensureFreezeState(player, menuData);

    menuData.Update();
  }

  internal static void input(CCSPlayerController player, MenuBase menu,
    PlayerButtons buttons) {
    if (getData(player.Slot) is not { } menuData) return;

    if (menuData.Menus.Count == 0 || menuData.Menus[0].Count == 0) return;

    for (var i = 0; i < MENU_BUTTONS.Length; i++) {
      var button     = MENU_BUTTONS[i];
      var buttonMask = menu.Options.Buttons[button];

      if (buttonMask == 0) continue;

      var isPressed = (buttons & buttonMask) != 0;

      var continuousDelay = menu.SelectedItem?.Item.Options.Continuous?[button]
        ?? menu.Options.Continuous[button];

      if (!isPressed) {
        menuData.LastInput[i] = 0;
        continue;
      }

      if (continuousDelay == 0 ?
        menuData.LastInput[i] != 0 :
        menuData.LastInput[i] + continuousDelay > Environment.TickCount64)
        continue;

      menuData.LastInput[i] = Environment.TickCount64;
      menu.input(button);
    }
  }

  internal static void remove(int playerSlot) { MENU_DATA[playerSlot] = null; }

  internal static MenuData? getData(int playerSlot) {
    return MENU_DATA[playerSlot];
  }

  internal static bool isSelectable(MenuItem menuItem) {
    return menuItem.Type is MenuItemType.CHOICE or MenuItemType.BUTTON
      or MenuItemType.INPUT;
  }
  
  private static void ensureFreezeState(CCSPlayerController player,
    MenuData menuData) {
    var anyFreezeMenu = menuData.Menus.Count > 0
      && menuData.Menus[0].Count > 0
      && menuData.Menus.Any(stack =>
        stack.Count > 0 && stack[^1].Options.BlockMovement);

    if (!anyFreezeMenu && player.IsAlive())
      player.Unfreeze();
  }
}