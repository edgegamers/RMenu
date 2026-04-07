using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;

namespace Example;

public partial class Example {
  private void example1Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    MenuValue header = new("new menu");
    MenuValue footer = new("footer",
      new MenuFormat(Color.Green, MenuStyle.BOLD));
    MenuBase menu = new(header, footer);

    Menu.Display(player, menu, callback: example1MenuCallback);
  }

  private void example1MenuCallback(MenuBase menu, MenuAction menuAction) {
    var player = menu.Player;

    switch (menuAction) {
      case MenuAction.START:
        player.PrintToChat("Menu Start");
        break;

      case MenuAction.EXIT:
        player.PrintToChat("Menu Exit");
        break;

      case MenuAction.SELECT:
        Menu.Clear(player);
        break;
    }
  }
}