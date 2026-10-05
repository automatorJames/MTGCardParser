namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a player pays: "{1}", "{w}{w}", "2 life".</summary>
[Dependent]
public class Cost : GlyphOneOf
{
    public ManaValue ManaValue { get; set; }
    public LifeQuantity LifeQuantity { get; set; }
}
