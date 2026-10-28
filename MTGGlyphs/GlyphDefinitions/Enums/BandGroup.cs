namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The group of creatures a "bands with other" keyword names: "bands with other legendary creatures", "bands with other creatures named wolves of the hunt", and the same group in its reminder text, "any legendary creatures can attack in a band …".</summary>
public enum BandGroup
{
    [RegexPattern("legendary creatures")]
    LegendaryCreatures,

    [RegexPattern("creatures named wolves of the hunt")]
    CreaturesNamedWolvesOfTheHunt
}
