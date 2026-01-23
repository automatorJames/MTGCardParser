namespace MTGPlexer.TokenEditor;

public abstract record EditorSnippet(
    string EditorRepresentation, 
    string ParameterRepresentation,
    bool DisplayAsBlockInEditor
    )
{
    public Guid Id { get; } = Guid.NewGuid();

    public abstract string GetParameterHtmlRepresentation();
    public abstract RegexSegmentBase GetRegexSegment();

    protected static string Span(string content, SpanClass spanClass = SpanClass.keyword) => $"<span class=\"{spanClass}\">{content}</span>";
}
