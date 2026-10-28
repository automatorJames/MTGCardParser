namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The counter kind in persist's and undying's reminder text, "-1/-1" or "+1/+1": "if it had no -1/-1 counters on it, return it to the battlefield under its owner's control with a -1/-1 counter on it".</summary>
public enum CounterSize
{
    [RegexPattern("-1/-1")]
    MinusOne,

    [RegexPattern(@"\+1/\+1")]
    PlusOne
}
