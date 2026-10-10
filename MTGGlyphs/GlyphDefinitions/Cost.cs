namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A price to pay: mana ("{u}"), tapping ("{t}"), life ("1 life"), sacrificing ("sacrifice {this}"), exiling ("exile {this}"), or removing counters ("remove a corpse counter from {this}").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>{u}</examplecapture>
[Dependent]
public class Cost : GlyphOneOf
{
    public ManaCost ManaCost { get; set; }
    public TapSymbol? TapSymbol { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
    public Sacrifice Sacrifice { get; set; }
    public DestroyTarget Exile { get; set; }
    public PlaceCounters RemoveCounters { get; set; }
}

public enum TapSymbol
{
    [RegexPattern(@"\{t\}")]
    Tap,

    [RegexPattern(@"\{q\}")]
    Untap,
}
