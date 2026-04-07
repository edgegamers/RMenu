using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;
using RMenu.Extensions;

namespace Example;

public partial class Example {
  private void example3Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    List<MenuObject> header = [
      new("new ", new MenuFormat(Color.Green)),
      new("menu", new MenuFormat(Color.Blue, MenuStyle.BOLD))
    ];

    List<MenuObject> footer = [
      new("footer",
        new MenuFormat(new Color().StrobeReversed(Color.Red, Color.Orange))),
      new(" extension", new MenuFormat(style: MenuStyle.ITALIC))
    ];

    MenuOptions options =
      new() { BlockMovement = true, DisplayItemsInHeader = true };
    options.Buttons[MenuButton.SELECT] = PlayerButtons.Use | PlayerButtons.Jump;

    MenuBase menu = new(header, footer, options);
    menu.Items.Add(new MenuItem(MenuItemType.BUTTON, new MenuValue("button")));

    Menu.Display(player, menu, callback: example3MenuCallback);
  }

  private void example3MenuCallback(MenuBase menu, MenuAction menuAction) {
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