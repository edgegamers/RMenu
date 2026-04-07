using System.Text;
using RMenu.Extensions;
using RMenu.Helpers;

namespace RMenu;

public class MenuObject(string text, MenuFormat? format = null) {
  internal string Display { get; set; } = text;
  public string Text { get; set; } = text;
  public MenuFormat Format { get; set; } = format ?? new MenuFormat();

  public static implicit operator MenuObject(string text) {
    return new MenuObject(text, new MenuFormat());
  }

  internal void render(StringBuilder stringBuilder,
    MenuFormat? highlight = null) {
    var format = Format;

    if (format.CanHighlight && highlight is not null) format = highlight;

    var color = format.Color;

    switch (color.A) {
      case 0:
        color = Rainbow.CurrentColor;
        break;

      case 1:
        Rainbow.Strobe(stringBuilder, Display, format);
        return;

      case 2:
        Rainbow.Strobe(stringBuilder, Display, format, true);
        return;
    }

    _ = stringBuilder.Append(
      $"<font class=\"{format.Style.Value()}\"><font color=\"#{color.R:X2}{color.G:X2}{color.B:X2}\">{Display}</font></font>");
  }
}