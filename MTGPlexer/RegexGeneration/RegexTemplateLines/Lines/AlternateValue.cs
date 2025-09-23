namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record AlternateValue
(
    Enclosure[] Enclosures,
    string Value,
    int Ordinal,
    int TotalOptions,
    string CommentPrefix = null
)
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        GetFormattedValue(Value, Ordinal),
        Comment: GetComment(Ordinal, TotalOptions, CommentPrefix)
    )
{
    static string GetFormattedValue(string value, int ordinal)
    {
        var firstChar = ordinal == 0 ? " " : "|";
        return firstChar + " " + value;
    }

    static string GetComment(int ordinal, int totalOptions, string commentPrefix = null)
    {
        var commentPostfix = $"{ordinal + 1}/{totalOptions}";

        if (string.IsNullOrEmpty(commentPrefix))
            return commentPostfix;
        else
            return $"{commentPrefix} : {commentPostfix}";
    }

    public override string ToString() => base.ToString();
}
