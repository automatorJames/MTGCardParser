namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that much life": an amount of life equal to a number just mentioned.</summary>
/// <exampledoc>Basilica Guards</exampledoc>
/// <examplecapture>that much life</examplecapture>
[Dependent]
public class ThatMuchLife : Glyph
{
    [RegexPattern("that much life")]
    public bool Text { get; set; }
}
