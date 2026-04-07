using CounterStrikeSharp.API.Core;
using RMenu.Enums;

namespace RMenu.Models;

public class MenuData(CCSPlayerController player) {
  internal readonly long[] LastInput =
    new long[Enum.GetValues(typeof(MenuButton)).Length];

  public CCSPlayerController Player { get; } = player;
  public List<List<MenuBase>> Menus { get; } = [];
  public (MenuBase Menu, string Html)? Current { get; set; }

  public void Update() {
    var currentTime = Environment.TickCount64;

    for (var i = 0; i < LastInput.Length; i++) LastInput[i] = currentTime;

    Current = null;
  }
}