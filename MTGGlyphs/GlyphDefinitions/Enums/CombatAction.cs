namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A combat action a creature can be made to take, forbidden or restricted: "attack", "block", "be blocked", "crew vehicles", as in "can't attack or block", "can't be blocked", "can't attack, block, or crew vehicles", "attacks each combat if able".</summary>
public enum CombatAction
{
    Attack,

    Block,

    [RegexPattern("be blocked")]
    BeBlocked,

    [RegexPattern("crew vehicles")]
    CrewVehicles
}
