namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The copula that asserts or denies something of a subject in a condition: "is", "isn't", "was", "wasn't", as in "{this} is tapped", "it isn't a mana ability", "enchanted artifact isn't a creature".</summary>
public enum Assertion
{
    Is,

    [RegexPattern("isn't")]
    Isnt,

    Was,

    [RegexPattern("wasn't")]
    Wasnt
}
