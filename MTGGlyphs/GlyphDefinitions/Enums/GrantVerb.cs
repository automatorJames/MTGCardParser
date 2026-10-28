namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb granting a quoted ability: "has"/"have" or "gains"/"gain", in `has "{t}: add one mana of any color"`, `gains trample and "{g}: regenerate this creature"`.</summary>
public enum GrantVerb
{
    [RegexPattern("has", "have")]
    Has,

    [RegexPattern("gains?")]
    Gains
}
