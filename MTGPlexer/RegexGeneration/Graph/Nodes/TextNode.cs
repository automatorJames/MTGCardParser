namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any TokenUnit property, but rather must be matched as part of the TokenUnit's overall Regex.
/// </summary>
public class TextNode : RegexNode
{
    public string Text { get; set; }

    public TextNode(RegexNode parentNode, Snippet snippet) 
        : base(parentNode, snippet.Text)
    {
        var text = snippet.Text;

        if (snippet.IsOptional)
            text = $"({text} )?";

        Text = text;
    }

    public override void AppendRegexBricks(RegexCollector collector) =>
        collector.Append(new RegexBrick(this, Text, "literal match"));
}