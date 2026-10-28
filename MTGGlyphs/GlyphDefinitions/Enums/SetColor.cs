namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A color setting that isn't one color: "all colors" ("{this} is all colors", "becomes all colors until end of turn") or "colorless" ("all slivers are colorless").</summary>
public enum SetColor
{
    [RegexPattern("all colors")]
    AllColors,

    Colorless
}
