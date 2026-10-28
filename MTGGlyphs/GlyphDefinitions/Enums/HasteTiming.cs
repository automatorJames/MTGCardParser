namespace MTGGlyphs.GlyphDefinitions;

/// <summary>When haste's reminder says a creature can attack and tap: "as soon as it comes under your control", "this turn", "no matter when it came under your control", "even if it just came under your control" (and the "they" forms).</summary>
public enum HasteTiming
{
    [RegexPattern("as soon as (it comes|they come) under your control")]
    AsSoonAsControlled,

    [RegexPattern("this turn")]
    ThisTurn,

    [RegexPattern("no matter when (it|they) came under your control")]
    NoMatterWhen,

    [RegexPattern("even if (it|they) just came under your control")]
    EvenIfJustControlled
}
