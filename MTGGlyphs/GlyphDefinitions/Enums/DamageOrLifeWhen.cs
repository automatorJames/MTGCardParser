namespace MTGGlyphs.GlyphDefinitions;

/// <summary>When counted damage or life happened: "this way" (by this effect) or "this turn", in "1 life you lost this turn", "1 life they paid this way".</summary>
public enum DamageOrLifeWhen
{
    [RegexPattern("this way")]
    ThisWay,

    [RegexPattern("this turn")]
    ThisTurn
}
