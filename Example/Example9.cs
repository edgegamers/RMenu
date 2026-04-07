using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;

namespace Example;

public partial class Example {
  private void example9Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    MenuBase menu = new();

    menu.Items.Add(new MenuItem(MenuItemType.INPUT,
      new MenuValue("Enter value: "), callback: (_, menuItem, menuAction)
        => {
        if (menuAction == MenuAction.INPUT && menuItem.Data is string input)
          player.PrintToChat($"Input - Data: {input}");
      }));

    Menu.Display(player, menu);
  }
}