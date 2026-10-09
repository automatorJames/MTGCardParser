namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A price to pay: mana ("{u}"), tapping ("{t}"), life ("1 life"), or sacrificing ("sacrifice {this}").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>{u}</examplecapture>
[Dependent]
public class Cost : GlyphOneOf
{
    public ManaCost ManaCost { get; set; }
    public TapSymbol? TapSymbol { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
    public Sacrifice Sacrifice { get; set; }
}

public enum TapSymbol
{
    [RegexPattern(@"\{t\}")]
    Tap,

    [RegexPattern(@"\{q\}")]
    Untap,
}
