using CounterStrikeSharp.API.Core;

namespace RMenu.Listeners;

internal static class OnTickListener {
  public static void Register() {
    NativeAPI.AddListener("OnTick", FunctionReference.Create(OnTick));
  }

  private static void OnTick() {
    for (var i = 0; i < Menu.MAX_PLAYERS; i++) {
      if (Menu.getData(i) is not { Current: { } current } menuData) continue;

      if (menuData.Player is not {
        IsValid : true, Connected: PlayerConnectedState.PlayerConnected
      } player) {
        Menu.remove(i);
        continue;
      }

      var result = Menu.RaiseOnPrintMenu(current.Menu, current.Html);
      player.PrintToCenterHtml(result, 1);
    }
  }
}