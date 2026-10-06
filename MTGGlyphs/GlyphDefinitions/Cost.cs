namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A price to pay: mana ("{u}") or life ("1 life").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>{u}</examplecapture>
public class Cost : GlyphOneOf
{
    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}

 