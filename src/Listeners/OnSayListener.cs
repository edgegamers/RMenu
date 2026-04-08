using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using RMenu.Enums;

namespace RMenu.Listeners;

internal static class OnSayListener {
  public static void Register() {
    NativeAPI.AddCommand("css_input", // your command
      "Menu input", false, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND,
      FunctionReference.Create(OnInputCommand));
  }


  private static void OnInputCommand(int playerSlot, IntPtr commandInfo) {
    var caller = playerSlot != -1 ?
      Utilities.GetPlayerFromSlot(playerSlot) :
      null;
    if (caller is not { IsValid        : true } player
      || Menu.Get(player) is not { Text: true } menu
      || menu.SelectedItem?.Item is not { } item)
      return;

    item.Data = NativeAPI.CommandGetArgByIndex(commandInfo, 1).Trim('"');

    menu.Text = false;
    menu.invoke(MenuAction.INPUT);
  }
}