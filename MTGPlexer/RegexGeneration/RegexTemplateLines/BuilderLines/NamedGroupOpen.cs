namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class NamedGroupOpen : EncloureBookend
{
    public NamedGroupOpen(Enclosure[] enclosures, string name, RegexPropInfo prop, string nameOverride = null)
        : base(
            enclosures,
            RenderCaptureGroup(prop, nameOverride),
            GetComment(prop)
        )
    {
    }

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