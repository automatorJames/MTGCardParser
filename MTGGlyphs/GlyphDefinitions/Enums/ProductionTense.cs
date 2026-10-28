namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether mana is matched by what a permanent could make or by what it just made: "could produce" ("any color that a land you control could produce"), "produced" ("any type that land produced").</summary>
public enum ProductionTense
{
    [RegexPattern("could produce")]
    CouldProduce,

    Produced
}
