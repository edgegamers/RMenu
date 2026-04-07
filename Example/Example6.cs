using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using RMenu;
using RMenu.Enums;
using RMenu.Extensions;

namespace Example;

public partial class Example {
  private static readonly
    List<( string mapName, string mapId, bool isLinear, int segments, int tier,
      float rating )> EXAMPLE6_DATA = [
      ("surf_longhop2_r_ljt_abc", "map_001", true, 5, 1, 4.2f),
      ("surf_skrillcake_r", "map_002", false, 8, 2, 3.8f),
      ("surf_comp_hopblocks_r_ljt_abc", "map_003", true, 3, 1, 4.5f),
      ("surf_synergy_x", "map_004", false, 12, 4, 4.7f),
      ("surf_aztectemple", "map_005", true, 7, 3, 3.9f),
      ("surf_minimountain", "map_006", false, 6, 2, 4.1f),
      ("surf_embrace", "map_007", true, 4, 1, 4.3f),
      ("surf_frozen_go", "map_008", false, 15, 5, 4.8f),
      ("surf_hopbhop", "map_009", true, 2, 1, 3.7f),
      ("surf_matrix_v2", "map_010", false, 10, 3, 4.4f),
      ("surf_nightmare", "map_011", true, 9, 4, 4.6f),
      ("surf_colors", "map_012", false, 5, 2, 3.6f),
      ("surf_factory", "map_013", true, 6, 2, 4.0f),
      ("surf_megabhop", "map_014", false, 20, 6, 4.9f),
      ("surf_toxic", "map_015", true, 8, 3, 4.2f)
    ];

  private void example6Menu(CCSPlayerController? player, CommandInfo info) {
    if (player is null || !player.IsValid) return;

    List<MenuObject> header = [
      "new", new("header", new MenuFormat(Color.Green))
    ];

    MenuValue footer = new("kzg",
      new MenuFormat(new Color().Strobe(Color.Red, Color.Purple)));
    MenuOptions options = new() {
      BlockMovement = true,
      DisplayItemsInHeader = true,
      Highlight = new MenuFormat { Color = Color.Green, Style = MenuStyle.BOLD }
    };

    MenuBase menu = new(header, footer, options);

    List<MenuValue> values = [];

    foreach (var (mapName, mapId, isLinear, segments, tier, rating) in
      EXAMPLE6_DATA) {
      MenuValue value   = new(mapName);
      var       objects = appendTail(isLinear, segments, tier, rating);

      value.Objects.AddRange(objects);
      value.Data = mapId;

      values.Add(value);
    }

    menu.Items.Add(new MenuItem(MenuItemType.CHOICE, values: values));
    menu.Items.Add(new MenuItem(MenuItemType.SPACER));

    foreach (var (mapName, mapId, isLinear, segments, tier, rating) in
      EXAMPLE6_DATA)
      menu.Items.Add(new MenuItem(MenuItemType.BUTTON, new MenuValue(mapName),
        tail: appendTail(isLinear, segments, tier, rating), data: mapId,
        options: new MenuItemOptions { Trim = MenuTrim.HEAD }));

    Menu.Display(player, menu, callback: example6MenuCallback);
  }

  private void example6MenuCallback(MenuBase menu, MenuAction menuAction) {
    var player = menu.Player;

    switch (menuAction) {
      case MenuAction.START:
        player.PrintToChat("Menu Start");
        break;

      case MenuAction.EXIT:
        player.PrintToChat("Menu Exit");
        break;

      case MenuAction.SELECT:
        var selectedItem = menu.SelectedItem?.Item;

        if (selectedItem is not null) {
          switch (selectedItem.Type) {
            case MenuItemType.BUTTON:
              player.PrintToChat($"Selected Map: {selectedItem.Data}");
              break;

            case MenuItemType.CHOICE:
              var selectedValue = selectedItem.SelectedValue?.Value;

              if (selectedValue is not null)
                player.PrintToChat($"Selected Map: {selectedValue.Data}");

              break;
          }

          if (selectedItem.Type == MenuItemType.BUTTON) Menu.Clear(player);
        }

        break;
    }
  }

  private static List<MenuObject> appendTail(bool isLinear, int segments,
    int tier, float rating) {
    List<MenuObject> objects = [
      " ",
      new($"{(isLinear ? "L" : $"S{segments}")} ",
        new MenuFormat(isLinear ? Color.DarkOrange : Color.Yellow,
          canHighlight: false)),
      new($"T{tier} ", new MenuFormat(tierToColor(tier), canHighlight: false)),
      new($"{rating:0.0}/5",
        new MenuFormat(style: MenuStyle.ITALIC, canHighlight: false))
    ];

    return objects;
  }

  private static Color tierToColor(int tier) {
    return tier switch {
      1 => new Color().Strobe(Color.LightBlue, Color.Aqua),
      2 => new Color().Strobe(Color.Blue, Color.DodgerBlue),
      3 => new Color().Strobe(Color.Purple, Color.MediumPurple),
      4 => new Color().Strobe(Color.Pink, Color.Magenta),
      5 => new Color().Strobe(Color.DarkRed, Color.Crimson),
      6 => new Color().Strobe(Color.Yellow, Color.Orange),
      _ => Color.White
    };
  }
}