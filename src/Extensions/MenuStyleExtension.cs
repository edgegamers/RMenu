using RMenu.Enums;

namespace RMenu.Extensions;

public static class MenuStyleExtension {
  public static string Value(this MenuStyle style) {
    return style switch {
      MenuStyle.NONE   => "stratum",
      MenuStyle.BOLD   => "stratum-bold",
      MenuStyle.ITALIC => "stratum-bold-italic",
      MenuStyle.MONO   => "stratum-bold-mono",
      _                => string.Empty
    };
  }
}