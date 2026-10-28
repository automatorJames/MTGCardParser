namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell narrowed by its caster not owning it: "you don't own" or "they don't own", as in "a spell you don't own".</summary>
public enum SpellOwnership
{
    [RegexPattern("(you|they) don't own")]
    NotOwned
}
