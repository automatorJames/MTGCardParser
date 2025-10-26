namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class NamedGroupOpen : EncloureBookend
{
    public NamedGroupOpen(Enclosure[] enclosures, RegexPropInfo prop)
        : base(
            enclosures,
            RenderCaptureGroup(prop),
            GetComment(prop)
        )
    {
    }

    static string RenderCaptureGroup(RegexPropInfo prop)
        => $"(?<{prop?.Name ?? ""}>";

    static string GetComment(RegexPropInfo prop)
    {
        var comment = prop.FriendlyTypeName;

        // Disambiguate the role of enum properties named differently than their types
        if (prop.RegexPropType == RegexPropType.Enum && prop.Name != prop.UnderlyingType.Name)
            comment += $": {prop.BaseType.Name}";

        return comment;
    }

    public override string ToString() => base.ToString();
}