namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class NamedGroupOpen : EnclosureBookend, IGroupOpen
{
    public string FullyQualifiedName { get; }
    public bool IsOptional { get; }

    public NamedGroupOpen(Enclosure[] enclosures, CaptureNode captureNode) : base(enclosures, RenderCaptureGroup(enclosures), GetComment(captureNode))
    {
        IsOptional = captureNode.IsOptional;
        FullyQualifiedName = GetFullyQualifiedName(enclosures);
    }

    static string RenderCaptureGroup(Enclosure[] enclosures) =>
        $"(?<{GetFullyQualifiedName(enclosures)}>";
    
    static string GetComment(CaptureNode captureNode)
    {
        var comment = captureNode.UnderlyingType.Name.ToFriendlyCase();

        // Disambiguate the role of enum properties named differently than their types
        bool enumPropHasDifferentNameFromTypeName =
            captureNode.UnderlyingType.IsEnum
            && captureNode.ConcreteProperty is PropertyInfo prop
            && prop.Name != captureNode.UnderlyingType.Name;

        if (enumPropHasDifferentNameFromTypeName)
            comment += $": {captureNode.UnderlyingType.Name}";

        return comment;
    }

    static string GetFullyQualifiedName(Enclosure[] enclosures) =>
        string.Join('_', enclosures.OfType<NamedEnclosure>().Select(x => x.Name));

    public override string ToString() => base.ToString();
}