namespace MTGPlexer.RegexGeneration.GraphNodes;

public class UnmatchedTextNode : Node
{
    public string Text { get; set; }

    public UnmatchedTextNode(Node parentNode, Snippet snippet) : base(parentNode, snippet.Text)
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