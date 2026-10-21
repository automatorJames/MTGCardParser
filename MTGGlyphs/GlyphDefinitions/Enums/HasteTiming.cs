namespace MTGGlyphs.GlyphDefinitions;

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
