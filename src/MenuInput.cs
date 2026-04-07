using CounterStrikeSharp.API;
using RMenu.Enums;

namespace RMenu;

public class MenuInput<T> where T : struct, Enum {
  private readonly PlayerButtons[] values =
    new PlayerButtons[Enum.GetValues(typeof(T)).Length];

  public MenuInput() {
    foreach (T button in Enum.GetValues(typeof(T)))
      values[Convert.ToUInt16(button)] = button switch {
        MenuButton.UP     => PlayerButtons.Forward,
        MenuButton.DOWN   => PlayerButtons.Back,
        MenuButton.LEFT   => PlayerButtons.Moveleft,
        MenuButton.RIGHT  => PlayerButtons.Moveright,
        MenuButton.SELECT => PlayerButtons.Jump,
        MenuButton.BACK   => PlayerButtons.Speed,
        MenuButton.EXIT   => PlayerButtons.Scoreboard,
        MenuButton.ASSIST => PlayerButtons.Inspect,
        _                 => 0
      };
  }

  public PlayerButtons this[T button] {
    get => values[Convert.ToUInt16(button)];
    set => values[Convert.ToUInt16(button)] = value;
  }
}