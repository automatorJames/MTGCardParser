using Glyphotype.RegexGeneration.Graph;

namespace Glyphotype.GlyphEditor;

public abstract record EditorNib
{
    public string EditorRepresentation { get; init; }
    public string ParameterRepresentation { get; init; }
    public string Id { get; init; }

    protected EditorNib(string editorRepresentation, string parameterRepresentation, string id)
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