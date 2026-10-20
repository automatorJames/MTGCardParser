namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Double strike's rule, as its reminder text puts it: "deals both first-strike and regular combat damage".</summary>
/// <exampledoc>Aven Sunstriker</exampledoc>
/// <examplecapture>deals both first-strike and regular combat damage</examplecapture>
[Dependent]
public class DealsBothCombatDamageSteps : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("deals?"), "both first-strike and regular combat damage"];
}
