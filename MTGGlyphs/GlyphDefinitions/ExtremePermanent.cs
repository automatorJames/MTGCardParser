namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The one permanent of a kind that's most extreme in some characteristic: "the creature with the least power", "the artifact with the greatest mana value", optionally allowing a tie ("the creature with the greatest power or tied for the greatest power").</summary>
/// <exampledoc>Triumph of Ferocity</exampledoc>
/// <examplecapture>the creature with the greatest power or tied for the greatest power</examplecapture>
[Dependent]
public class ExtremePermanent : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Kind), Prop(Extreme), Prop(OrTied)];

    public CardType Kind { get; set; }
    public WithExtremeCharacteristic Extreme { get; set; }
    [Optional]
    [RegexPattern("or tied for the (?:greatest|least|highest|lowest) (?:power|toughness|mana value)")]
    public bool? OrTied { get; set; }
}
