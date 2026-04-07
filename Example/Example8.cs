using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;

namespace Example;

public partial class Example {
  private void example8Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    MenuBase menu = new();

    menu.Items.Add(new MenuItem(MenuItemType.BUTTON,
      values: [
        .. Enumerable.Range(1, 100).Select(i => new MenuValue(i.ToString()))
      ],
      options: new MenuItemOptions {
        Pinwheel = false,
        Continuous = new MenuContinuous<MenuButton> {
          [MenuButton.LEFT] = 50, [MenuButton.RIGHT] = 50
        }
      }));

    menu.Items.Add(new MenuItem(MenuItemType.BUTTON,
      values: [
        .. Enumerable.Range(1, 100).Select(i => new MenuValue(i.ToString()))
      ],
      options: new MenuItemOptions {
        Pinwheel = false,
        Continuous = new MenuContinuous<MenuButton> {
          [MenuButton.LEFT] = 500, [MenuButton.RIGHT] = 500
        }
      }));

    Menu.Display(player, menu);
  }
}