namespace Glyphotype.GlyphEditor;

public record EditorTextNib : EditorNib
{
    public string RawText { get; init; }
    public string TrimmedText { get; init; }

    public EditorTextNib(string text, string id)
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

    public override NamedGroupNode ToNamedGroupNode()
    {
        // new TextSegment(TrimmedText);
        throw new NotImplementedException();
    }

    
}