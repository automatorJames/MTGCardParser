namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One thing mana can be spent on: casting spells of a kind, activating abilities (optionally of permanents of a kind), paying a keyword's cost ("pay cumulative upkeep costs"), turning permanents face up, or unlocking doors.</summary>
/// <exampledoc>Renowned Weaponsmith</exampledoc>
/// <examplecapture>activate abilities of artifacts</examplecapture>
[Dependent]
public class ManaUse : GlyphOneOf
{
    public CastSpellsUse Cast { get; set; }
    public ActivateAbilitiesUse Activate { get; set; }
    public PayKeywordCostUse PayKeywordCost { get; set; }
    public TurnFaceUpUse TurnFaceUp { get; set; }
    public UnlockDoorUse UnlockDoor { get; set; }
}
