using CounterStrikeSharp.API.Core;
using RMenu.Enums;
using RMenu.Models;

namespace RMenu;

public class MenuBase(MenuValue? header = null, MenuValue? footer = null,
  MenuOptions? options = null, object? data = null) {
  public CCSPlayerController Player { get; set; } = null!;
  public List<MenuItem> Items { get; set; } = [];
  public MenuSelectedItem? SelectedItem { get; set; }
  public MenuValue? Header { get; set; } = header;
  public MenuValue? Footer { get; set; } = footer;
  public MenuOptions Options { get; set; } = options ?? new MenuOptions();
  public object? Data { get; set; } = data;
  public int CurrentPage { get; internal set; }

  internal Action<MenuBase, MenuAction>? Callback { get; set; }
  internal bool Text { get; set; }

  internal int PageCount
    => Options is { Paginate: true, AvailableItems: > 0 } ?
      Math.Max(1,
        (int)Math.Ceiling(Items.Count / (double)Options.AvailableItems)) :
      1;

  internal void input(MenuButton button) {
    Action? action = button switch {
      MenuButton.UP     => handleUp,
      MenuButton.DOWN   => handleDown,
      MenuButton.LEFT   => handleLeft,
      MenuButton.RIGHT  => handleRight,
      MenuButton.SELECT => handleSelect,
      MenuButton.BACK   => handleBack,
      MenuButton.EXIT   => handleExit,
      MenuButton.ASSIST => handleAssist,
      _                 => null
    };

    action?.Invoke();
  }

  internal void invoke(MenuAction menuAction) {
    var menuItem  = SelectedItem?.Item;
    var menuValue = menuItem?.SelectedValue?.Value;

    menuValue?.Callback?.Invoke(this, menuValue, menuAction);
    menuItem?.Callback?.Invoke(this, menuItem, menuAction);
    Callback?.Invoke(this, menuAction);
  }

  private void handleUp() {
    if (SelectedItem?.Index is not { } index || Text) return;

    for (var newIndex = index - 1; newIndex >= 0; newIndex--)
      if (isSelected(newIndex)) {
        invoke(MenuAction.CHOOSE);
        return;
      }
  }

  private void handleDown() {
    if (SelectedItem?.Index is not { } index || Text) return;

    for (var newIndex = index + 1; newIndex < Items.Count; newIndex++)
      if (isSelected(newIndex)) {
        invoke(MenuAction.CHOOSE);
        return;
      }
  }

  private void handleLeft() {
    if (SelectedItem?.Item is not { } menuItem || Text) return;

    if (menuItem.Input(MenuButton.LEFT)) {
      invoke(MenuAction.UPDATE);
      return;
    }

    if (Options.Paginate) navigatePage(-1);
  }

  private void handleRight() {
    if (SelectedItem?.Item is not { } menuItem || Text) return;

    if (menuItem.Input(MenuButton.RIGHT)) {
      invoke(MenuAction.UPDATE);
      return;
    }

    if (Options.Paginate) navigatePage(1);
  }

  private void handleSelect() {
    if (SelectedItem?.Item is not { } menuItem) return;

    if (menuItem.Type == MenuItemType.INPUT)
      Text = true;
    else
      invoke(MenuAction.SELECT);
  }

  private void handleBack() {
    if (SelectedItem?.Item is { Type: MenuItemType.INPUT } && Text)
      Text = false;
    else if (Menu.Close(Player)) invoke(MenuAction.EXIT);
  }

  private void handleExit() {
    if (!Options.Exitable) return;

    Menu.Clear(Player);
    invoke(MenuAction.EXIT);
  }

  private void handleAssist() { invoke(MenuAction.ASSIST); }

  private bool isSelected(int index) {
    return Menu.isSelectable(Items[index])
      && (SelectedItem = new MenuSelectedItem(index, Items[index])) != null;
  }

  private void navigatePage(int delta) {
    var newPage = Math.Clamp(CurrentPage + delta, 0, PageCount - 1);
    if (newPage == CurrentPage) return;

    CurrentPage = newPage;

    var pageSize = Options.AvailableItems;
    var start    = CurrentPage * pageSize;
    var end      = Math.Min(Items.Count, start + pageSize);

    for (var i = start; i < end; i++)
      if (isSelected(i)) {
        invoke(MenuAction.CHOOSE);
        return;
      }
  }
}