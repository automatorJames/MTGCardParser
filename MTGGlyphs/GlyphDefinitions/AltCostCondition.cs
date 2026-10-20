namespace MTGGlyphs.GlyphDefinitions;

/// <summary>When an alternative cost may be used: "if you or a teammate has cast another spell this turn" (surge), "if an opponent lost life this turn" (spectacle), "if you dealt combat damage to a player this turn with a goblin or rogue" (prowl).</summary>
/// <exampledoc>Boulder Salvo</exampledoc>
/// <examplecapture>if you or a teammate has cast another spell this turn</examplecapture>
[Dependent]
public class AltCostCondition : GlyphOneOf
{
    [RegexPattern("if you or a teammate has cast another spell this turn")]
    public bool Surge { get; set; }
    public IfHappenedThisTurn ThisTurn { get; set; }
    public ProwlCondition Prowl { get; set; }
}
