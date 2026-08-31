namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any Glyph property, but rather must be matched as part of the Glyph's overall Regex.
/// </summary>
public class TextNode : RegexNode
{
    /// <summary>
    /// Punctuation that binds tight to whatever token sits next to it, so no space is ever written on that
    /// side of it. Drives both halves of how a joiner is placed around a text nib, from this one set:
    /// <see cref="StartsWithTightPunctuation"/> suppresses the joiner that would otherwise come *before* the
    /// nib (nothing separates a token from the comma or <c>'s</c> that follows it), and
    /// <see cref="EndsWithTightPunctuation"/> makes the joiner *after* the nib adhere to it rather than
    /// standing on its own (see <c>RegexBrickFormattingPipeline.FoldAdheringJoiners</c>) - so a nib written
    /// <c>","</c> renders and reads as <c>,[ ]</c>, exactly as if it had been written <c>", "</c>.
    /// </summary>
    static readonly HashSet<char> _tightPunctuation = ['\'', ',', '.', ';', ':', '!', '?'];

    /// <summary>The literal regex text to match, wrapped as optional (e.g. <c>(text )?</c>) if the source nib was optional, with every literal space escaped to <see cref="BuiltRegex.EscapedSpace"/>.</summary>
    public string Text { get; set; }

    public char FirstChar => Text.First();
    public char LastChar => Text.Last();

    /// <summary>Whether this node's text opens with <see cref="_tightPunctuation"/> - e.g. <c>'s</c>, or a bare <c>","</c> nib - and so must hug the token before it rather than be separated from it by a joiner.</summary>
    public bool StartsWithTightPunctuation => _tightPunctuation.Contains(FirstChar);

    /// <summary>Whether this node's text closes with <see cref="_tightPunctuation"/>, so the joiner that follows it belongs on this node's own line rather than a line of its own.</summary>
    public bool EndsWithTightPunctuation => _tightPunctuation.Contains(LastChar);

    /// <summary>Whether this node's text already opens with a space of its own, which a joiner in front of it would double up with. The mirror of <see cref="RegexCollector.AlreadySeparated"/>, which guards the same thing from the other side.</summary>
    public bool StartsWithSpace => Text.StartsWith(BuiltRegex.EscapedSpace) || FirstChar == ' ';

    /// <summary>
    /// Whether the joiner that would otherwise precede this node should be dropped - because the node opens
    /// with punctuation that binds to the token before it, or because it already supplies that space itself.
    /// Together with <see cref="RegexCollector.AlreadySeparated"/> this is what makes a comma nib authored as
    /// <c>","</c>, <c>", "</c> or <c>" , "</c> all come out as the same single-spaced pattern.
    /// </summary>
    public bool AbsorbsPrecedingJoiner => StartsWithTightPunctuation || StartsWithSpace;

    public TextNode(RegexNode parentNode, Nib nib)
        : base(parentNode, nib.Text)
    {
        var text = nib.Text;

        if (string.IsNullOrEmpty(text))
            throw new Exception($"{nameof(TextNode)} text can't be null or empty");

        if (nib.IsOptional)
            text = $"({text} )?";

        // Escaped here rather than left as a raw space so that "is there already a space here?" (see
        // RegexCollector.AlreadySeparated) is one check against one token, whether the space came from a nib
        // or from a joiner - and so that a nib's own spaces survive IgnorePatternWhitespace if it's ever
        // enabled. BuiltRegex unescapes the whole pattern before compiling, so matching is unaffected.
        // EscapeSpaces is idempotent, so a nib that already spells its space out as "[ ]" (the same token
        // this produces, and the same thing that token means as a regex) lands on exactly the text it would
        // have if it had been written with a plain space.
        Text = BuiltRegex.EscapeSpaces(text);
    }

    protected override void AppendOwnRegexBricks(RegexCollector collector) =>
        collector.Append(new RegexBrick(this, Text));
}