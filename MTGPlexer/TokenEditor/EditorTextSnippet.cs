namespace MTGPlexer.TokenEditor;

public record EditorTextSnippet : EditorSnippet
{
    public string Text { get; init; }

    public EditorTextSnippet(string text, string id)
        : base(
            editorRepresentation: text,
            parameterRepresentation: $"\"{text}\"",
            id: id)
    {
        Text = text;
    }

    public override string GetParameterHtmlRepresentation() =>
        Span($"\"{Text}\"", SpanClass.stringliteral);

    public override RegexSegmentBase GetRegexSegment() =>
        new TextSegment(Text);
}