
namespace MTGPlexer.CommonDTOs;

public record EditorTextSnippet(string Text)
    : EditorSnippet(
        EditorRepresentation: Text,
        ParameterRepresentation: $"\"{Text}\"",
        DisplayAsBlockInEditor: false)
{
    public override string GetParameterHtmlRepresentation() =>
        Span("\"" + Text + "\"", SpanClass.stringliteral);

    public override RegexSegmentBase GetRegexSegment() =>
        new TextSegment(Text);
}
