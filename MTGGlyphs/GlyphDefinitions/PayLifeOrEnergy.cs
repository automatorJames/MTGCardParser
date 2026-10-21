namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Paying life or energy as a cost, with the verb: "pay 1 life", "pay {e}", "pay eight {e}", "pay {e}{e} (two energy counters)", or as a gerund after "by": "paying 3 life".</summary>
/// <exampledoc>Mana Confluence</exampledoc>
/// <examplecapture>pay 1 life</examplecapture>
[Dependent]
public class PayLifeOrEnergy : Glyph
{
    public override Nib[] Nibs => [Alt("pay", "paying"), Prop(Amount), Prop(Note)];

    public OneOf<LifeQuantity, ManaCost, EnergyAmount> Amount { get; set; }
    [Optional]
    public EnergyNote Note { get; set; }
}
