using CounterStrikeSharp.API.Core;

namespace Example;

public partial class Example : BasePlugin {
  public override string ModuleName => "Example";
  public override string ModuleVersion => "1.0.0";

  public override void Load(bool hotReload) {
    AddCommand("css_example1", "", example1Menu);
    AddCommand("css_example2", "", example2Menu);
    AddCommand("css_example3", "", example3Menu);
    AddCommand("css_example4", "", example4Menu);
    AddCommand("css_example5", "", example5Menu);
    AddCommand("css_example6", "", example6Menu);
    AddCommand("css_example7", "", example7Menu);
    AddCommand("css_example8", "", example8Menu);
    AddCommand("css_example9", "", example9Menu);
  }
}