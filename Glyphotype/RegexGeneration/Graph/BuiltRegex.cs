namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// The compiled result of walking a <see cref="RegexNode"/> graph: the flat brick sequence, the
/// concatenated matching pattern, and the compiled <see cref="System.Text.RegularExpressions.Regex"/>.
/// Formatted/commented output is a separate concern — see <see cref="ToSmartRegex"/>.
/// </summary>
public class BuiltRegex
{
    readonly List<RegexBrick> _regexBricks;

    /// <summary>The flat, unformatted brick sequence this regex was compiled from — the raw input to <see cref="RegexBrickFormattingPipeline.Format"/>.</summary>
    public List<RegexBrick> Bricks => _regexBricks;

    /// <summary>The concatenated raw regex text of every brick, used to compile <see cref="Regex"/>.</summary>
    public string MinifiedRegex { get; }

    /// <summary>The compiled matching pattern.</summary>
    public Regex Regex { get; }

    public BuiltRegex(List<RegexBrick> regexBricks)
    {
        _regexBricks = regexBricks;
        MinifiedRegex = string.Join("", _regexBricks.Select(x => x.Regex)).Replace("[ ]", " ");
        Regex = new(MinifiedRegex, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }

    /// <summary>Builds the formatted, colorized, commented representation of this regex for human-readable output.</summary>
    public SmartRegex ToSmartRegex(GlyphOccurrenceSummary summary, RegexGraph regexGraph, bool includeSupplementalLines = true) =>
        new(_regexBricks, summary, regexGraph, includeSupplementalLines);

    public override string ToString() => MinifiedRegex;
}
