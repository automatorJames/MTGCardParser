namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class NamedGroupOpen : EnclosureBookend, IGroupOpen
{
    public string FullyQualifiedName { get; }
    public bool IsOptional { get; }

    public NamedGroupOpen(Enclosure[] enclosures, TemplatePropInfo prop, bool isOptional = false) : base(enclosures, RenderCaptureGroup(enclosures), GetComment(prop))
    {
        IsOptional = isOptional || prop.Prop.IsDefined(typeof(OptionalComponentAttribute));
        FullyQualifiedName = GetFullyQualifiedName(enclosures);
    }

    static string RenderCaptureGroup(Enclosure[] enclosures) =>
        $"(?<{GetFullyQualifiedName(enclosures)}>";
    
    static string GetComment(TemplatePropInfo prop)
    {
        var comment = prop.GetFriendlyTypeName();

        // Disambiguate the role of enum properties named differently than their types
        if (prop.TemplatePropType == TemplatePropType.Enum && prop.Name != prop.UnderlyingType.Name)
            comment += $": {prop.UnderlyingType.Name}";

        return comment;
    }

    static string GetFullyQualifiedName(Enclosure[] enclosures) =>
        string.Join('_', enclosures.OfType<NamedEnclosure>().Select(x => x.Name));

    public override string ToString() => base.ToString();
}