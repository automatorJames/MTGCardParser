namespace MTGGlyphs.GlyphDefinitions;

public enum CreatureKeywordAction
{
    [RegexPattern("explores")]
    Explore,

    [RegexPattern("connives")]
    Connive,

    [RegexPattern("endures")]
    Endure,

    [RegexPattern("scries")]
    Scry,

    [RegexPattern("surveils")]
    Surveil,

    [RegexPattern("investigates")]
    Investigate,

    [RegexPattern("manifests dread")]
    ManifestDread,

    [RegexPattern("proliferates")]
    Proliferate,

    [RegexPattern("discovers")]
    Discover,

    [RegexPattern("amasses goblins", "amasses orcs", "amasses zombies")]
    Amass
}
