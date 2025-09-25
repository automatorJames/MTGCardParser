namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record NamedGroupOpen
(
    Enclosure[] Enclosures,
    string Name,
    RegexPropInfo Prop,
    string NameOverride = null
)
    : EncloureBookend
    (
        Enclosures: Enclosures,
        Regex: RenderCaptureGroup(Prop, NameOverride),
        Comment: GetComment(Prop)
    )
{
    static string RenderCaptureGroup(RegexPropInfo prop, string nameOverride)
        => $"(?<{prop?.Name ?? nameOverride ?? ""}>";

    static string GetComment(RegexPropInfo prop)
    {
        var comment = prop.FriendlyTypeName;

        // Disambiguate the role of enum properties named differently than their types
        if (prop.RegexPropType == RegexPropType.Enum && prop.Name != prop.UnderlyingType.Name)
                comment += $": {prop.UnderlyingType.Name}";

        return comment;
    }

    public override string ToString() => base.ToString();
}