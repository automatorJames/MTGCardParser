namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a"/"an" or "another", opening any one permanent, spell or card of a kind: "a land", "another creature you control", "a red spell", "another card in their hand".</summary>
public enum IndefiniteDeterminer
{
    [RegexPattern("an?")]
    A,

    Another
}
