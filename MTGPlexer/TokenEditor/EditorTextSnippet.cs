
namespace MTGPlexer.CommonDTOs;

public record EditorTextSnippet(string Text, string Id)
    : EditorSnippet(
        EditorRepresentation: Text,
        ParameterRepresentation: $"\"{Text}\"",
        DisplayAsBlockInEditor: false,
        Id)
{
    public override string GetParameterHtmlRepresentation() =>
        Span("\"" + Text + "\"", SpanClass.stringliteral);

    public override RegexSegmentBase GetRegexSegment() =>
        new TextSegment(Text);
}
