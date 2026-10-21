namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One thing mana can be spent on: casting spells of a kind, or activating abilities (optionally of permanents of a kind).</summary>
/// <exampledoc>Renowned Weaponsmith</exampledoc>
/// <examplecapture>activate abilities of artifacts</examplecapture>
[Dependent]
public class ManaUse : GlyphOneOf
{
    public CastSpellsUse Cast { get; set; }
    public ActivateAbilitiesUse Activate { get; set; }
}
