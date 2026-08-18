namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Central, easily-tweakable font family for formatted regex output, applied to the whole block. There is
/// deliberately no per-role alternate font here: <see cref="CommentBoxMetrics"/> and every padding/column
/// calculation around it measure text by character count, which only lines up on screen if every span
/// shares one truly monospace font. Nothing outside this class should hardcode a font family for formatted
/// output. Sits alongside <see cref="SmartRegexStaticRules"/> (layout) and <see cref="SmartSpanControlPanel"/>
/// (color) as the third "knobs" class for formatted regex output.
/// </summary>
public static class SmartFontConfig
{
    /// <summary>The font family for all formatted regex output.</summary>
    public static string PrimaryFontFamily = "'Fira Code', monospace";
}
