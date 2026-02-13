using MTGPlexer.RegexGeneration.Graph;

namespace MTGPlexer.TokenEditor;

public abstract record EditorSnippet
{
    public string EditorRepresentation { get; init; }
    public string ParameterRepresentation { get; init; }
    public string Id { get; init; }

    protected EditorSnippet(string editorRepresentation, string parameterRepresentation, string id)
    {
        EditorRepresentation = editorRepresentation;
        ParameterRepresentation = parameterRepresentation;
        Id = id;
    }

    public abstract string GetParameterHtmlRepresentation();
    public abstract NamedGroupNode ToNamedGroupNode();

    protected static string Span(string content, SpanClass spanClass = SpanClass.keyword)
        => $"<span class=\"{spanClass}\">{content}</span>";
}