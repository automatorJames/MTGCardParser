namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action as the third-person verb its subject performs: "explores", "connives", "scries", "surveils", "manifests dread", "amasses zombies" - "it explores", "target player scries 3".</summary>
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
