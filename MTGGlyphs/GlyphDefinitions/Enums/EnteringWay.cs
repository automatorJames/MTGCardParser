namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb by which a permanent comes onto the battlefield in some state: "enters" (or "enter"), or "escapes" when it comes by escape - "enters tapped", "escapes with a +1/+1 counter on it".</summary>
public enum EnteringWay
{
    [RegexPattern("enters", "enter")]
    Enters,

    [RegexPattern("escapes")]
    Escapes
}
