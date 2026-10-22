namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Buyback's and kicker's reminder text: "you may pay an additional {3} as you cast this spell", "you may pay an additional {2}{u} and/or {1}{b} as you cast this spell".</summary>
/// <exampledoc>Capsize</exampledoc>
/// <examplecapture>you may pay an additional {3} as you cast this spell</examplecapture>
public class BuybackRule : Glyph
{
    public override Nib[] Nibs => ["you may pay an additional", Prop(Cost), Prop(AndOr), "as you cast this spell"];

    public ManaCost Cost { get; set; }
    [Optional]
    public AndOrCost AndOr { get; set; }
}
