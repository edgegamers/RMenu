using System.Drawing;
using RMenu.Enums;
using RMenu.Extensions;

namespace RMenu;

public class MenuOptions {
  private readonly HashSet<string> options = [];
  private bool blockMovement;

  private MenuInput<MenuButton> buttons = new();
  private MenuContinuous<MenuButton> continuous = new();

  private MenuObject[] cursor = [
    new("►", new MenuFormat(new Color().Rainbow())),
    new("◄", new MenuFormat(new Color().Rainbow()))
  ];

  private bool displayItemsInHeader = true;
  private bool exitable = true;
  private MenuFontSize footerFontSize = MenuFontSize.S;

  private MenuFontSize headerFontSize = MenuFontSize.L;
  private MenuFormat? highlight;

  private MenuValue input = new("________");
  private MenuFontSize itemFontSize = MenuFontSize.SM;
  private int priority;

  private bool processInput = true;
  private bool paginate;

  private MenuObject[] selector = [
    new("[ ", new MenuFormat(new Color().Rainbow())),
    new(" ]", new MenuFormat(new Color().Rainbow()))
  ];

  public MenuOptions() { updateHtml(); }

  public MenuOptions(MenuOptions source) {
    headerFontSize = source.headerFontSize;
    itemFontSize   = source.itemFontSize;
    footerFontSize = source.footerFontSize;

    processInput         = source.processInput;
    blockMovement        = source.blockMovement;
    displayItemsInHeader = source.displayItemsInHeader;
    exitable             = source.exitable;
    priority             = source.priority;
    paginate             = source.paginate;

    buttons    = new MenuInput<MenuButton>();
    continuous = new MenuContinuous<MenuButton>();
    cursor     = new MenuObject[source.cursor.Length];
    selector   = new MenuObject[source.selector.Length];

    foreach (var button in Enum.GetValues<MenuButton>())
      buttons[button] = source.buttons[button];

    foreach (var button in Enum.GetValues<MenuButton>())
      continuous[button] = source.continuous[button];

    for (var i = 0; i < source.cursor.Length; i++) {
      var original = source.cursor[i];

      cursor[i] = new MenuObject(original.Text,
        new MenuFormat(original.Format.Color, original.Format.Style,
          original.Format.CanHighlight));
    }

    for (var i = 0; i < source.selector.Length; i++) {
      var original = source.selector[i];

      selector[i] = new MenuObject(original.Text,
        new MenuFormat(original.Format.Color, original.Format.Style,
          original.Format.CanHighlight));
    }

    input = source.input;

    if (source.highlight is not null)
      highlight = new MenuFormat(source.highlight.Color,
        source.highlight.Style, source.highlight.CanHighlight);

    options = [.. source.options];
    updateHtml();
  }

  public MenuFontSize HeaderFontSize {
    get => headerFontSize;
    set {
      headerFontSize = value;
      _               = options.Add(nameof(HeaderFontSize));
      updateHtml();
    }
  }

  public MenuFontSize ItemFontSize {
    get => itemFontSize;
    set {
      itemFontSize = value;
      _             = options.Add(nameof(ItemFontSize));
      updateHtml();
    }
  }

  public MenuFontSize FooterFontSize {
    get => footerFontSize;
    set {
      footerFontSize = value;
      _               = options.Add(nameof(FooterFontSize));
      updateHtml();
    }
  }

  public MenuInput<MenuButton> Buttons {
    get => buttons;
    set {
      buttons = value;
      _        = options.Add(nameof(Buttons));
    }
  }

  public MenuContinuous<MenuButton> Continuous {
    get => continuous;
    set {
      continuous = value;
      _           = options.Add(nameof(Continuous));
    }
  }

  public bool ProcessInput {
    get => processInput;
    set {
      processInput = value;
      _             = options.Add(nameof(ProcessInput));
    }
  }

  public bool BlockMovement {
    get => blockMovement;
    set {
      blockMovement = value;
      _              = options.Add(nameof(BlockMovement));
    }
  }

  public bool DisplayItemsInHeader {
    get => displayItemsInHeader;
    set {
      displayItemsInHeader = value;
      _                     = options.Add(nameof(DisplayItemsInHeader));
    }
  }

  public bool Exitable {
    get => exitable;
    set {
      exitable = value;
      _         = options.Add(nameof(Exitable));
    }
  }

  public int Priority {
    get => priority;
    set {
      priority = value;
      _         = options.Add(nameof(Priority));
    }
  }
  
  public bool Paginate {
    get => paginate;
    set {
      paginate = value;
      _         = options.Add(nameof(Paginate));
      updateHtml();
    }
  }

  public MenuObject[] Cursor {
    get => cursor;
    set {
      cursor = value;
      _       = options.Add(nameof(Cursor));
    }
  }

  public MenuObject[] Selector {
    get => selector;
    set {
      selector = value;
      _         = options.Add(nameof(Selector));
    }
  }

  public MenuValue Input {
    get => input;
    set {
      input = value;
      _      = options.Add(nameof(Input));
    }
  }

  public MenuFormat? Highlight {
    get => highlight;
    set {
      highlight = value;
      _          = options.Add(nameof(Highlight));
    }
  }

  internal string HeaderSizeHtml { get; private set; } = string.Empty;
  internal string ItemSizeHtml { get; private set; } = string.Empty;
  internal string FooterSizeHtml { get; private set; } = string.Empty;
  internal int AvailableChars { get; private set; } = 1;
  internal int AvailableItems { get; private set; } = 1;

  internal void merge(MenuOptions overrides) {
    if (overrides.isSet(nameof(HeaderFontSize)))
      HeaderFontSize = overrides.HeaderFontSize;

    if (overrides.isSet(nameof(ItemFontSize)))
      ItemFontSize = overrides.ItemFontSize;

    if (overrides.isSet(nameof(FooterFontSize)))
      FooterFontSize = overrides.FooterFontSize;

    if (overrides.isSet(nameof(ProcessInput)))
      ProcessInput = overrides.ProcessInput;

    if (overrides.isSet(nameof(BlockMovement)))
      BlockMovement = overrides.BlockMovement;

    if (overrides.isSet(nameof(DisplayItemsInHeader)))
      DisplayItemsInHeader = overrides.DisplayItemsInHeader;

    if (overrides.isSet(nameof(Exitable))) Exitable = overrides.Exitable;

    if (overrides.isSet(nameof(Priority))) Priority = overrides.Priority;
    
    if (overrides.isSet(nameof(Paginate))) Paginate = overrides.Paginate;

    if (overrides.isSet(nameof(Buttons))) Buttons = overrides.Buttons;

    if (overrides.isSet(nameof(Continuous))) Continuous = overrides.Continuous;

    if (overrides.isSet(nameof(Cursor))) Cursor = overrides.Cursor;

    if (overrides.isSet(nameof(Selector))) Selector = overrides.Selector;

    if (overrides.isSet(nameof(Input))) Input = overrides.Input;

    if (overrides.isSet(nameof(Highlight))) Highlight = overrides.Highlight;
  }

  private bool isSet(string propertyName) {
    return options.Contains(propertyName);
  }

  private void updateHtml() {
    HeaderSizeHtml =
      $"<font class=\"fontSize-{headerFontSize.ToString().ToLower()}\">";
    ItemSizeHtml =
      $"<font class=\"fontSize-{itemFontSize.ToString().ToLower()}\">";
    FooterSizeHtml =
      $"<font class=\"fontSize-{footerFontSize.ToString().ToLower()}\">";

    var availableHeight = Menu.MENU_HEIGHT
      - ((int)HeaderFontSize + (int)FooterFontSize);
    
    if (paginate) availableHeight -= (int)ItemFontSize;

    AvailableChars = (int)(Menu.MENU_LENGTH / ((int)ItemFontSize * 0.6)
      - (Cursor[0].Display.Length + Cursor[1].Display.Length
        + Selector[0].Display.Length + Selector[1].Display.Length));

    AvailableItems = Math.Max(1, availableHeight / (int)ItemFontSize);
  }
}