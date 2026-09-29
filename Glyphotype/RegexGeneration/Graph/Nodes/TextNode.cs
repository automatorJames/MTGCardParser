namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any Glyph property, but rather must be matched as part of the Glyph's overall Regex.
/// </summary>
public class TextNode : RegexNode
{
    /// <summary>The literal regex text to match, wrapped as optional (e.g. <c>(text )?</c>) if the source nib was optional, with every literal space escaped to <see cref="BuiltRegex.EscapedSpace"/>.</summary>
    public string Text { get; set; }

    /// <summary>
    /// Whether this nib was authored as a bare period - the one way a Glyph type declares that it
    /// deliberately spans a clause boundary (see <see cref="RegexGraph.SpansClauses"/>). Checked against
    /// the authored nib text rather than <see cref="Text"/>, since by then the period has been escaped to
    /// <c>\.</c> and a pattern nib like <c>zzg.*</c> would otherwise look the same as a literal period.
    /// </summary>
    public bool IsClauseBreak { get; }

    /// <summary>Whether this nib is an <see cref="OptionalPluralNib"/> - a suffix of the word before it (e.g. the "s" of "dogs"), so never separated from that word (see <see cref="JoinerRules.Between"/>).</summary>
    public bool IsPluralSuffix { get; }

    public TextNode(RegexNode parentNode, Nib nib)
        : base(parentNode, nib.Text)
    {
        var text = nib.Text;

        if (string.IsNullOrEmpty(text))
            throw new Exception($"{nameof(TextNode)} text can't be null or empty");

        // A bare "." nib means a literal period, but "." is regex for "any character" - left unescaped a
        // clause-spanning type would happily match any character where it declared a clause break.
        IsClauseBreak = text == ".";
        IsPluralSuffix = nib is OptionalPluralNib;

        if (IsClauseBreak)
            text = @"\.";

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