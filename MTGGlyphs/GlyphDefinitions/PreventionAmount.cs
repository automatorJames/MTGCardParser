namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How much damage a prevention effect stops: "the next 3", or "all".</summary>
/// <exampledoc>Healing Salve</exampledoc>
/// <examplecapture>the next 3</examplecapture>
[Dependent]
public class PreventionAmount : GlyphOneOf
{
    public NextDamage Next { get; set; }
    [RegexPattern("all")]
    public bool All { get; set; }
}
