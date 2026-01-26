namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class NamedGroupOpen : EnclosureBookend, IGroupOpen
{
    public string FullyQualifiedName { get; }
    public bool IsOptional { get; }

    public NamedGroupOpen(Enclosure[] enclosures, TemplatePropInfo prop) : base(enclosures, RenderCaptureGroup(enclosures), GetComment(prop))
    {
        IsOptional = 
            prop.Proptions.HasFlag(Proptions.Optional) 
            || prop.Prop.IsDefined(typeof(OptionalComponentAttribute))
            || prop.TemplatePropType == TemplatePropType.Enum && Nullable.GetUnderlyingType(prop.Prop.PropertyType) != null;

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