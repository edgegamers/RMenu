using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;
using RMenu.Extensions;

namespace Example;

public partial class Example {
  private void example2Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    List<MenuObject> header = [
      new("new ", new MenuFormat(Color.Green)),
      new("menu", new MenuFormat(Color.Blue, MenuStyle.BOLD))
    ];

    MenuValue footer = new("footer",
      new MenuFormat(new Color().Strobe(Color.Red, Color.Orange)));

    MenuBase menu = new(header, footer);

    Menu.Display(player, menu, callback: (_, menuAction) => {
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
    });
  }
}