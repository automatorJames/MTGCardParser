namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with mana value 3 or less", "each with mana value 2 or less": a ceiling on the mana value of the card(s) sought - "each" when several are sought and each must meet it.</summary>
/// <exampledoc>Cateran Kidnappers</exampledoc>
/// <examplecapture>with mana value 3 or less</examplecapture>
[Dependent]
public class ManaValueLimit : Glyph
{
    public override Nib[] Nibs => [Prop(Each), "with mana value", Prop(Amount), "or", Alt("less", "more")];

    [Optional]
    [RegexPattern("each")]
    public bool? Each { get; set; }
    public Quantity Amount { get; set; }
}
