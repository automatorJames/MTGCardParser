
using MTGPlexer.RegexGeneration.Graph;

namespace MTGPlexer.TokenEditor;

public record EditorTextSnippet : EditorSnippet
{
    public string RawText { get; init; }
    public string TrimmedText { get; init; }

    public EditorTextSnippet(string text, string id)
        : base(
            editorRepresentation: text,
            parameterRepresentation: $"\"{text.Trim()}\"",
            id: id)
    {
        RawText = text;
        TrimmedText = text.Trim();
    }

    public override string GetParameterHtmlRepresentation() =>
        Span($"\"{TrimmedText}\"", SpanClass.stringliteral);

    public override CaptureNode ToCaptureNode()
    {
        // new TextSegment(TrimmedText);
        throw new NotImplementedException();
    }

    
}