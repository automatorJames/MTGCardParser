namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with mana value 3 or less": a ceiling on the mana value of the card sought.</summary>
/// <exampledoc>Cateran Kidnappers</exampledoc>
/// <examplecapture>with mana value 3 or less</examplecapture>
[Dependent]
public class ManaValueLimit : Glyph
{
    public override Nib[] Nibs => ["with mana value", Prop(Amount), "or", Alt("less", "more")];

    public Quantity Amount { get; set; }
}
