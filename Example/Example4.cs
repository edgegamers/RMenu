using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;

namespace Example;

public partial class Example {
  private static readonly List<(string, string, int)> EXAMPLE4_DATA = [
    ("Option", "1", 1), ("Option", "2", 2), ("Option", "3", 3)
  ];

  private void example4Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    MenuOptions options = new() {
      BlockMovement = true,
      Cursor = [
        new MenuObject("-", new MenuFormat(Color.Yellow)),
        new MenuObject("-", new MenuFormat(Color.Yellow))
      ],
      Selector       = [new MenuObject("( "), new MenuObject(" )")],
      Highlight      = new MenuFormat(Color.Green, MenuStyle.BOLD),
      HeaderFontSize = MenuFontSize.S,
      ItemFontSize   = MenuFontSize.L,
      FooterFontSize = MenuFontSize.SM
    };

    MenuBase menu = new(new MenuValue("header"), new MenuValue("footer"),
      options);

    foreach (var (head, tail, data) in EXAMPLE4_DATA)
      menu.Items.Add(new MenuItem(MenuItemType.BUTTON,
        new MenuValue($"{head} "),
        tail: new MenuValue(tail, new MenuFormat(Color.Red, MenuStyle.BOLD)),
        data: data, callback: Example4ItemCallback));

    menu.Items.Add(new MenuItem(MenuItemType.SPACER));

    foreach (var (head, tail, data) in EXAMPLE4_DATA)
      menu.Items.Add(new MenuItem(MenuItemType.BUTTON,
        new MenuValue($"{head} "),
        tail: new MenuValue(tail, new MenuFormat(Color.Blue, MenuStyle.BOLD)),
        data: data, callback: (_, menuItem, menuAction) => {
          switch (menuAction) {
            case MenuAction.SELECT:
              player.PrintToChat($"Select - Data: {menuItem.Data}");
              break;
            case MenuAction.UPDATE:
              player.PrintToChat($"Update - Data: {menuItem.Data}");
              break;
          }
        }));

    Menu.Display(player, menu, callback: Example4MenuCallback);
  }

  private void Example4ItemCallback(MenuBase menu, MenuItem menuItem,
    MenuAction menuAction) {
    var player = menu.Player;

    if (menuAction == MenuAction.SELECT)
      player.PrintToChat($"Select - Data: {menuItem.Data}");

    if (menuAction == MenuAction.UPDATE)
      player.PrintToChat($"Update - Data: {menuItem.Data}");
  }

  private void Example4MenuCallback(MenuBase menu, MenuAction menuAction) {
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