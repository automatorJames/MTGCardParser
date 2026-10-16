namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Paying life or energy as a cost, with the verb: "pay 1 life", "pay {e}".</summary>
/// <exampledoc>Mana Confluence</exampledoc>
/// <examplecapture>pay 1 life</examplecapture>
[Dependent]
public class PayLifeOrEnergy : Glyph
{
    public override Nib[] Nibs => ["pay", Prop(Amount)];

    public OneOf<LifeQuantity, ManaCost> Amount { get; set; }
}
