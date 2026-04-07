using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using RMenu.Enums;

namespace RMenu.Listeners;

internal static class OnSayListener {
  public static void Register() {
    NativeAPI.HookUsermessage(118,
      (InputArgument)FunctionReference.Create(OnSay), HookMode.Pre);
  }

  private static HookResult OnSay(UserMessage um) {
    var index   = um.ReadInt("entityindex");
    var message = um.ReadString("param2");

    if (Utilities.GetPlayerFromIndex(index) is not { IsValid: true } player
      || Menu.Get(player) is not { Text                     : true } menu
      || menu.SelectedItem?.Item is not { } item)
      return HookResult.Continue;

    item.Data = message;

    menu.Text = false;
    menu.invoke(MenuAction.INPUT);

    return HookResult.Continue;
  }
}