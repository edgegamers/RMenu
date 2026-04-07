using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;

namespace Example;

public partial class Example {
  private static readonly List<(string, string, int)> EXAMPLE5_DATA = [
    ("Option ", "1", 1), ("Option ", "2", 2), ("Option trailing ", "3", 3)
  ];

  private void example5Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    MenuOptions options =
      new() { BlockMovement = true, DisplayItemsInHeader = false };
    MenuBase menu = new(new MenuValue("header"), new MenuValue("footer"),
      options);

    List<MenuValue> values = [];

    foreach (var (head, tail, data) in EXAMPLE5_DATA) {
      var value = formatValue(head, tail);
      value.Data = data;

      values.Add(value);
    }

    menu.Items.Add(new MenuItem(MenuItemType.BUTTON, new MenuValue("Select: "),
      values));

    menu.Items.Add(new MenuItem(MenuItemType.BUTTON,
      new MenuValue("verylonghead verylonghead", new MenuFormat(Color.Blue)),
      tail: new MenuValue(" short tail", new MenuFormat(Color.Red)),
      options: new MenuItemOptions { Trim = MenuTrim.HEAD }));

    menu.Items.Add(new MenuItem(MenuItemType.BUTTON,
      new MenuValue("short head", new MenuFormat(Color.Blue)),
      tail: new MenuValue(" verylongtail verylongtail",
        new MenuFormat(Color.Red)),
      options: new MenuItemOptions { Trim = MenuTrim.TAIL }));

    Menu.Display(player, menu, callback: example5MenuCallback);
  }

  private static MenuValue formatValue(string head, string tail) {
    List<MenuObject> value = [
      new($"{head} ", new MenuFormat(Color.Blue, MenuStyle.BOLD)),
      new(tail, new MenuFormat(Color.Red, MenuStyle.BOLD))
    ];

    return value;
  }

  private void example5MenuCallback(MenuBase menu, MenuAction menuAction) {
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