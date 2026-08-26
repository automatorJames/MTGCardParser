namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any Glyph property, but rather must be matched as part of the Glyph's overall Regex.
/// </summary>
public class TextNode : RegexNode
{
    /// <summary>The literal regex text to match, wrapped as optional (e.g. <c>(text )?</c>) if the source nib was optional.</summary>
    public string Text { get; set; }

    public char FirstChar => Text.First();

    public TextNode(RegexNode parentNode, Nib nib) 
        : base(parentNode, nib.Text)
    {
        var text = nib.Text;

        if (string.IsNullOrEmpty(text))
            throw new Exception($"{nameof(TextNode)} text can't be null or empty");

        if (nib.IsOptional)
            text = $"({text} )?";

        Text = text;
    }

    protected override void AppendOwnRegexBricks(RegexCollector collector) =>
        collector.Append(new RegexBrick(this, Text));
}