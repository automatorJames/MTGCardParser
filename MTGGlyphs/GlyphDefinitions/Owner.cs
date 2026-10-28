namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whose zone, library, hand, life total or choice it is: a plain possessive ("your", "their", "its owner's", "an opponent's", "that player's") or a targeted player's ("target player's", "target opponent's"). The one possessor type for every "{player}'s {thing}" construction.</summary>
/// <exampledoc>Clairvoyance</exampledoc>
/// <examplecapture>target player's</examplecapture>
[Dependent]
public class Owner : GlyphOneOf
{
    public Whose? Whose { get; set; }
    public PlayerPossessive Targeted { get; set; }
}
