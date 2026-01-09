namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class NamedGroupOpen : EncloureBookend, IGroupOpen
{
    public string FullyQualifiedName { get; }
    public bool IsOptional { get; }

    public NamedGroupOpen(Enclosure[] enclosures, RegexPropInfo prop) : base(enclosures, RenderCaptureGroup(enclosures), GetComment(prop))
    {
        IsOptional = prop.Prop.IsDefined(typeof(OptionalComponentAttribute));
        FullyQualifiedName = GetFullyQualifiedName(enclosures);
    }

    static string RenderCaptureGroup(Enclosure[] enclosures) =>
        $"(?<{GetFullyQualifiedName(enclosures)}>";
    
    static string GetComment(RegexPropInfo prop)
    {
        var comment = prop.FriendlyTypeName;

        // Disambiguate the role of enum properties named differently than their types
        if (prop.RegexPropType == RegexPropType.Enum && prop.Name != prop.UnderlyingType.Name)
            comment += $": {prop.BaseType.Name}";

        return comment;
    }

    static string GetFullyQualifiedName(Enclosure[] enclosures) =>
        string.Join('_', enclosures.OfType<NamedEnclosure>().Select(x => x.Name));

    public override string ToString() => base.ToString();
}