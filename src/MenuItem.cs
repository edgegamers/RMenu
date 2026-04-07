using RMenu.Enums;
using RMenu.Models;

namespace RMenu;

public class MenuItem {
  private List<MenuValue>? values;

  public MenuItem(MenuItemType type, MenuValue? head = null,
    List<MenuValue>? values = null, MenuValue? tail = null,
    MenuItemOptions? options = null, object? data = null,
    Action<MenuBase, MenuItem, MenuAction>? callback = null) {
    Type     = type;
    Head     = head;
    Values   = values;
    Tail     = tail;
    Options  = options ?? new MenuItemOptions();
    Data     = data;
    Callback = callback;
  }

  public MenuItemType Type { get; set; }
  public MenuValue? Head { get; set; }
  public MenuValue? Tail { get; set; }
  public MenuItemOptions Options { get; init; }
  public object? Data { get; set; }
  public Action<MenuBase, MenuItem, MenuAction>? Callback { get; }
  public MenuSelectedValue? SelectedValue { get; set; }

  public List<MenuValue>? Values {
    get => values;
    set {
      values       = value;
      SelectedValue = null;

      if (values is { Count: > 0 })
        SelectedValue = new MenuSelectedValue(0, values[0]);
    }
  }

  public bool Input(MenuButton button) {
    if (values is not { Count: > 0 }) return false;

    SelectedValue ??= new MenuSelectedValue(0, values[0]);

    var newIndex = button switch {
      MenuButton.LEFT => Options.Pinwheel ?
        (SelectedValue.Index - 1 + values.Count) % values.Count :
        Math.Max(0, SelectedValue.Index - 1),
      MenuButton.RIGHT => Options.Pinwheel ?
        (SelectedValue.Index + 1) % values.Count :
        Math.Min(values.Count - 1, SelectedValue.Index + 1),
      _ => SelectedValue.Index
    };

    if (newIndex == SelectedValue.Index) return false;

    SelectedValue = new MenuSelectedValue(newIndex, values[newIndex]);
    return true;
  }
}