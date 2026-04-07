using System.Text;
using RMenu.Enums;

namespace RMenu;

public class MenuValue {
  public MenuValue(string text, MenuFormat? format = null, object? data = null,
    Action<MenuBase, MenuValue, MenuAction>? callback = null) {
    Objects  = [new MenuObject(text, format)];
    Data     = data;
    Callback = callback;
  }

  public MenuValue(IEnumerable<MenuObject> values, object? data = null,
    Action<MenuBase, MenuValue, MenuAction>? callback = null) {
    Objects  = [.. values];
    Data     = data;
    Callback = callback;
  }

  public List<MenuObject> Objects { get; set; }
  public object? Data { get; set; }
  public Action<MenuBase, MenuValue, MenuAction>? Callback { get; }

  public static implicit operator MenuValue(List<MenuObject> menuObjects) {
    return new MenuValue(menuObjects);
  }

  internal void render(StringBuilder stringBuilder,
    MenuFormat? highlight = null) {
    foreach (var t in Objects) t.render(stringBuilder, highlight);
  }

  internal int calculateLength(MenuFormat? highlight = null) {
    double length = 0;

    foreach (var t in Objects) {
      var    objectFormat = t.Format;
      double objectLength = t.Text.Length;

      if (objectFormat.CanHighlight && highlight is not null)
        objectFormat = highlight;

      if (objectFormat.Style == MenuStyle.MONO) objectLength *= 1.2;

      length += objectLength;
    }

    return (int)length;
  }
}