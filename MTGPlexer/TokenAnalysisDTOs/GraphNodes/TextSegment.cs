namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any TokenUnit property, but rather must be matched as part of the TokenUnit's overall Regex.
/// </summary>
public record TextNode : Node
{
    public string Text { get; set; }
    public bool DoNotAddPrecedingSpace { get; }

    public TextNode(Node parentNode, Snippet snippet) : base(parentNode, snippet.Text)
    {
        var text = snippet.Text;

        if (snippet.IsOptional)
            text = $"({text} )?";

        Text = text;
        DoNotAddPrecedingSpace = snippet.IsNoPrecedingSpace;
    }

    public override void ComposeRegexLines(RegexBuilder builder) =>
        builder.AddTextLine(Text, doNotAddPrecedingSpace: DoNotAddPrecedingSpace);
}