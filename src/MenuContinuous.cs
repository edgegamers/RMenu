using RMenu.Enums;

namespace RMenu;

public class MenuContinuous<T> where T : struct, Enum {
  private readonly int[] values = new int[Enum.GetValues(typeof(T)).Length];

  public MenuContinuous() {
    foreach (T button in Enum.GetValues(typeof(T)))
      values[Convert.ToUInt16(button)] = button switch {
        MenuButton.UP    => 150,
        MenuButton.DOWN  => 150,
        MenuButton.LEFT  => 150,
        MenuButton.RIGHT => 150,
        _                => 0
      };
  }

  public int this[T button] {
    get => values[Convert.ToUInt16(button)];
    set => values[Convert.ToUInt16(button)] = value;
  }
}