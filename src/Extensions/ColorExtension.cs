using System.Drawing;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace RMenu.Extensions;

public static class ColorExtension {
  public static Color Rainbow(this Color _) {
    return Color.FromArgb(0, 0, 0, 0);
  }

  public static Color Strobe(this Color _, byte hueDelta = 60) {
    return Color.FromArgb(1, 0, 255, clampHue(hueDelta));
  }

  public static Color Strobe(this Color _, Color startColor, Color endColor,
    byte hueDelta = 60) {
    return Color.FromArgb(1, colorToByte(startColor), colorToByte(endColor),
      clampHue(hueDelta));
  }

  public static Color StrobeReversed(this Color _, byte hueDelta = 60) {
    return Color.FromArgb(2, 0, 255, clampHue(hueDelta));
  }

  public static Color StrobeReversed(this Color _, Color startColor,
    Color endColor, byte hueDelta = 60) {
    return Color.FromArgb(2, colorToByte(startColor), colorToByte(endColor),
      clampHue(hueDelta));
  }

  private static byte clampHue(byte hueDelta) {
    return (byte)(hueDelta < 1 ? 1 : hueDelta);
  }

  private static byte colorToByte(Color color) {
    var r = color.R / 255f;
    var g = color.G / 255f;
    var b = color.B / 255f;

    var max   = Math.Max(r, Math.Max(g, b));
    var min   = Math.Min(r, Math.Min(g, b));
    var delta = max - min;

    var hue = 0f;

    if (delta != 0f) {
      if (max == r)
        hue = (g - b) / delta % 6f;
      else if (max == g)
        hue = (b - r) / delta + 2f;
      else
        hue = (r - g) / delta + 4f;
    }

    hue *= 60f;

    if (hue < 0f) hue += 360f;

    return (byte)(hue / Helpers.Rainbow.HUE_BYTE);
  }
}