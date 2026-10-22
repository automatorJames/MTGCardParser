namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What happened to the counted life or damage: passively ("prevented this way", "dealt to you this turn") or by a player ("you lost", "your opponents have lost this turn").</summary>
/// <exampledoc>Neheb, the Eternal</exampledoc>
/// <examplecapture>your opponents have lost this turn</examplecapture>
[Dependent]
public class DamageOrLifeEvent : GlyphOneOf
{
    public PlayerChangedLife ByPlayer { get; set; }
    public DamageOrLifeHappened Passive { get; set; }
}
