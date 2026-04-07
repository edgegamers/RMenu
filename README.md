# RMenu

> A rich, feature-packed HTML center-menu library for [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) plugins.

---

## Massive, Enormous, Colossal Shoutout to [oscar-wos](https://github.com/oscar-wos)

> **This library would not exist without [oscar-wos](https://github.com/oscar-wos).**
>
> Every single piece of the core architecture, the rendering pipeline, the HTML engine, the input hooking system, the rainbow/strobe colour system, the menu threading model, the trim logic, the selector and cursor system **all of it was built by oscar-wos from the ground up.**
>
> We are standing on the shoulders of his work. We forked this project and added a handful of quality-of-life features to suit our own needs.
> 
> If you use this library, go star his repo. He earned it.

---

## Overview

RMenu is the **[EdgeGamers](https://edgm.rs) standard menu library** for CounterStrikeSharp plugins. It provides a fully-featured, highly customisable HTML-rendered center-screen menu system driven by player button inputs no chat commands required.

## Installation

### NuGet
```
dotnet add package RMenu
```
Or add it directly to your `.csproj`:
```xml
<PackageReference Include="RMenu" Version="*" />
```

---

## Quick Start
```csharp
using RMenu;
using RMenu.Enums;

// Display a basic menu to a player
MenuBase menu = new(new MenuValue("My Menu"), new MenuValue("footer"));
menu.Items.Add(new MenuItem(MenuItemType.BUTTON, new MenuValue("Option 1")));
menu.Items.Add(new MenuItem(MenuItemType.BUTTON, new MenuValue("Option 2")));

Menu.Display(player, menu, callback: (m, action) => {
if (action == MenuAction.SELECT)
player.PrintToChat($"You selected item {m.SelectedItem?.Index + 1}");

    if (action == MenuAction.EXIT)
        player.PrintToChat("Menu closed");
});
```
---

## Core Concepts

### `MenuBase`

The root menu object. Holds header, footer, items, options, and the player it is displayed to.
```csharp
MenuBase menu = new(
header:  new MenuValue("Header Text"),
footer:  new MenuValue("Footer Text"),
options: new MenuOptions { BlockMovement = true },
data:    someOptionalData
);
```
### `MenuValue` & `MenuObject`

A `MenuValue` is a piece of displayable text, made up of one or more `MenuObject` segments — each with their own colour and style.
```csharp
// Simple value
MenuValue simple = new("Hello World");

// Styled value
MenuValue styled = new("Warning!", new MenuFormat(Color.Red, MenuStyle.BOLD));

// Multi-segment value (implicit from List<MenuObject>)
MenuValue multipart = [
new("Hello ", new MenuFormat(Color.Green)),
new("World",  new MenuFormat(Color.Blue, MenuStyle.BOLD))
];
```
### `MenuFormat`

Controls the appearance of a `MenuObject`.
```csharp
new MenuFormat(
color:        Color.Cyan,
style:        MenuStyle.BOLD,   // NONE | BOLD | ITALIC | MONO
canHighlight: true              // Whether the selection highlight overrides this format
)
```
### Colour Effects

Use the `RMenu.Extensions` colour extensions for animated effects:
```csharp
using RMenu.Extensions;

// Animated rainbow cycle
new MenuFormat(new Color().Rainbow())

// Strobe between two colours
new MenuFormat(new Color().Strobe(Color.Red, Color.Orange))

// Reversed strobe
new MenuFormat(new Color().StrobeReversed(Color.Blue, Color.Purple))
```
---

## Menu Items

Add items to `menu.Items` using `MenuItem`:
```csharp
new MenuItem(
type:     MenuItemType.BUTTON,
head:     new MenuValue("Label "),
values:   null,                         // scrollable values (CHOICE / multi-BUTTON)
tail:     new MenuValue(" suffix"),
options:  new MenuItemOptions(),
data:     someData,
callback: (menu, item, action) => { }   // per-item callback
)
```
### Item Types

| Type | Description |
|---|---|
| `BUTTON` | A selectable button. Add `values` to make it a scrollable picker. |
| `CHOICE` | A standalone scrollable value picker. |
| `INPUT` | A text input field — activates chat input mode on select. |
| `TEXT` | Non-selectable display text. |
| `SPACER` | A blank line separator. |

### `MenuItemOptions`
```csharp
new MenuItemOptions {
Pinwheel  = true,          // Whether the value list wraps around at the ends
Trim      = MenuTrim.HEAD, // NONE | HEAD | TAIL — truncate overflow from head or tail
Continuous = new MenuContinuous<MenuButton> {
[MenuButton.LEFT]  = 100,  // ms hold-repeat delay for LEFT on this item
[MenuButton.RIGHT] = 100
}
}
```
---

## Menu Options

`MenuOptions` controls the look and behaviour of an entire menu.
```csharp
MenuOptions options = new() {
BlockMovement        = true,                 // Freeze player movement
Exitable             = true,                 // Allow EXIT button to close menu
DisplayItemsInHeader = true,                 // Show "2/5 ⇦" counter in header
Priority             = 0,                    // Higher priority menus insert at front
HeaderFontSize       = MenuFontSize.L,
ItemFontSize         = MenuFontSize.SM,
FooterFontSize       = MenuFontSize.S,
Highlight            = new MenuFormat(Color.Green, MenuStyle.BOLD),
Cursor               = [
new MenuObject("► ", new MenuFormat(Color.Yellow)),
new MenuObject(" ◄", new MenuFormat(Color.Yellow))
],
Selector             = [new MenuObject("[ "), new MenuObject(" ]")],
Input                = new MenuValue("________"),  // Placeholder for INPUT items
};

// Remap buttons
options.Buttons[MenuButton.SELECT] = PlayerButtons.Use | PlayerButtons.Jump;

// Set continuous hold delay globally
options.Continuous[MenuButton.UP]   = 150;
options.Continuous[MenuButton.DOWN] = 150;
```
### Font Sizes

`MenuFontSize`: `XS` `S` `SM` `M` `ML` `L` `XL` `XXL` `XXXL`

---

## Menu Actions

Callbacks receive a `MenuAction` value:

| Action | Description |
|---|---|
| `START` | Menu was just displayed to the player |
| `SELECT` | Player confirmed the current selection |
| `CHOOSE` | Player moved the cursor to a different item |
| `UPDATE` | Player changed a scrollable value (LEFT/RIGHT) |
| `EXIT` | Player exited the menu via BACK or EXIT |
| `ASSIST` | Player pressed the ASSIST button |
| `INPUT` | Player submitted text via chat (INPUT items) |

---

## Static `Menu` API
```csharp
// Display a menu to a player
Menu.Display(player, menu, subMenu: false, callback: (m, a) => { });

// Get the currently active menu for a player
MenuBase? current = Menu.Get(player);

// Close the top sub-menu and return to the previous
Menu.Close(player);

// Clear all menus for a player
Menu.Clear(player);

// Force-clear even non-exitable menus
Menu.Clear(player, force: true);
```
---

## Sub-Menus

Pass `subMenu: true` to push a menu on top of the existing stack. The sub-menu inherits the parent's options (with overrides merged in) and shows the `⇦` back indicator in the header.
```
csharp
Menu.Display(player, subMenu: true, menu: childMenu, callback: (m, action) => {
if (action == MenuAction.EXIT)
player.PrintToChat("Returned from sub-menu");
});
```
---

## Text Input Items

Add an `INPUT` item to collect free-text from a player via chat:
```csharp
menu.Items.Add(new MenuItem(MenuItemType.INPUT, new MenuValue("Name: ")));

Menu.Display(player, menu, callback: (m, action) => {
if (action == MenuAction.INPUT) {
var item  = m.SelectedItem?.Item;
var input = item?.Data as string;
player.PrintToChat($"You typed: {input}");
}
});
```
When the player selects the INPUT item, the placeholder is shown and the next chat message they send is captured as the value — without printing to chat.

---

## `OnPrintMenu` Event

Intercept and modify the raw HTML string before it is sent to the player each tick:
```csharp
Menu.OnPrintMenu += (_, e) => {
e.Html = e.Html.Replace("someText", "replacedText");
};
```
---

## Per-Item Callbacks

Each `MenuItem` and `MenuValue` can have its own callback in addition to the top-level menu callback. All three are fired in order: value → item → menu.
```csharp
menu.Items.Add(new MenuItem(
MenuItemType.BUTTON,
new MenuValue("Delete"),
data: playerId,
callback: (menu, item, action) => {
if (action == MenuAction.SELECT)
player.PrintToChat($"Deleted: {item.Data}");
}
));
```
---

## Default Button Bindings

| Menu Action | Default CS2 Button |
|---|--------------------|
| UP | `Forward` (W)      |
| DOWN | `Back` (S)         |
| LEFT | `Moveleft` (A)     |
| RIGHT | `Moveright` (D)    |
| SELECT | `Jump` (Space)     |
| BACK | `Duck` (Ctrl)      |
| EXIT | `Scoreboard` (Tab) |

---

> *Built for the [EdgeGamers](https://edgm.rs) community. All credit for the foundational work again goes to [oscar-wos](https://github.com/oscar-wos) — an absolute legend.*
```
