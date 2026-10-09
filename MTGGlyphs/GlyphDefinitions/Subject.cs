namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who or what does something: a target, the enchanted permanent, a player ("you", "each opponent"), or the card itself.</summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>target player</examplecapture>
[Dependent]
public class Subject : GlyphOneOf
{
    public SpecificTarget SpecificTarget { get; set; }
    public EnchantedPermanent EnchantedPermanent { get; set; }
    public WhichPlayer? Player { get; set; }
    public This This { get; set; }
}
