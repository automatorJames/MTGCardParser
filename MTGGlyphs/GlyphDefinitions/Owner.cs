namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose zone it is: a plain possessive ("your", "their", "an opponent's") or a targeted player's ("target player's", "defending player's").</summary>
/// <exampledoc>Clairvoyance</exampledoc>
/// <examplecapture>target player's</examplecapture>
[Dependent]
public class Owner : GlyphOneOf
{
    public TargetPlayersPossessive Targeted { get; set; }
    public Whose? Whose { get; set; }
}
