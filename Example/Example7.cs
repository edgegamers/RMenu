using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;

namespace Example;

public partial class Example {
  private static readonly List<MenuObject> HEADER = [
    "new", new("header", new MenuFormat(Color.Green))
  ];

  private static readonly MenuOptions OPTIONS = new() {
    BlockMovement = true,
    DisplayItemsInHeader = true,
    Highlight = new MenuFormat { Color = Color.Green, Style = MenuStyle.BOLD },
    ItemFontSize = MenuFontSize.S
  };

  private void example7Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    MenuBase menu = new(HEADER, options: OPTIONS);

    menu.Items.Add(new MenuItem(MenuItemType.BUTTON,
      values: [
        new MenuValue("sub menu", callback: subMenuCallback),
        new MenuValue("sub menu2", callback: subMenu2Callback)
      ], options: new MenuItemOptions { Pinwheel = false }));

    Menu.Display(player, menu);
  }

  private void subMenuCallback(MenuBase menu, MenuValue menuValue,
    MenuAction menuAction) {
    if (menuAction != MenuAction.SELECT) return;

    MenuBase subMenu = new(HEADER);

    subMenu.Items.Add(new MenuItem(MenuItemType.TEXT,
      new MenuValue("sub menu text, use shift (walk)")));

    Menu.Display(menu.Player, subMenu, true);
  }

  private void subMenu2Callback(MenuBase menu, MenuValue menuValue,
    MenuAction menuAction) {
    if (menuAction != MenuAction.SELECT) return;

    MenuOptions options = new() {
      Highlight = new MenuFormat { Color = Color.Blue, Style = MenuStyle.BOLD }
    };

    MenuBase subMenu = new(HEADER, options: options);

    subMenu.Items.Add(new MenuItem(MenuItemType.BUTTON,
      new MenuValue("back button"), callback: (menuBase, _, action) => {
        if (action == MenuAction.SELECT) Menu.Close(menuBase.Player);
      }));

    Menu.Display(menu.Player, subMenu, true);
  }
}