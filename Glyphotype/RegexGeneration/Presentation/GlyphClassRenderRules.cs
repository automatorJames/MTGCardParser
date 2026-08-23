namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Tunable knobs for <see cref="GlyphClassRenderer"/>'s output - the "C# Class" footer tray view's own
/// small knobs class, alongside <see cref="SmartRegexStaticRules"/> (formatted regex layout) and
/// <see cref="SmartSpanControlPanel"/> (formatted regex color).
/// </summary>
public static class GlyphClassRenderRules
{
    /// <summary>Spaces of indentation for a class body's members (the Nibs line, attribute lines, property lines) from the class's own braces.</summary>
    public const int BodyIndentSpaces = 4;

    /// <summary>
    /// Resting brightness (0..1) for neutral C# keyword text - <c>public</c>, <c>class</c>, <c>override</c>,
    /// <c>Prop(</c>, <c>get</c>, <c>set</c> - the lightest of the three neutral shades, since keywords read
    /// as the most "sentence-like" of the uninteresting text.
    /// </summary>
    public const double NeutralKeywordBrightness = 0.62;

    /// <summary>Resting brightness (0..1) for neutral brace/bracket/generic-angle-bracket text (<c>{ } [ ] &lt; &gt; ( )</c>), including the quotes around a Nibs-array literal.</summary>
    public const double NeutralBraceBrightness = 0.45;

    /// <summary>Resting brightness (0..1) for neutral fine punctuation (<c>; :</c>) - the darkest of the three neutral shades, since it's the least meaningful.</summary>
    public const double NeutralPunctuationBrightness = 0.32;
}
