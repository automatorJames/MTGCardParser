using MTGPlexer.RegexGeneration.GraphNodes;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class NamedGroupOpen : EnclosureBookend, IGroupOpen
{
    public string FullyQualifiedName { get; }
    public bool IsOptional { get; }

    public NamedGroupOpen(Enclosure[] enclosures, CaptureNode captureNode) : base(enclosures, RenderCaptureGroup(enclosures), GetComment(captureNode))
    {
        IsOptional =
            captureNode.Proptions.HasFlag(Proptions.Optional)
            || captureNode.PropertySnippet.Prop.IsDefined(typeof(OptionalComponentAttribute))
            || captureNode.UnderlyingType.IsEnum && Nullable.GetUnderlyingType(captureNode.PropertySnippet.Prop.PropertyType) != null;

        FullyQualifiedName = GetFullyQualifiedName(enclosures);
    }

    static string RenderCaptureGroup(Enclosure[] enclosures) =>
        $"(?<{GetFullyQualifiedName(enclosures)}>";
    
    static string GetComment(CaptureNode captureNode)
    {
        var comment = captureNode.UnderlyingType.Name.ToFriendlyCase();

        // Disambiguate the role of enum properties named differently than their types
        if (captureNode.UnderlyingType.IsEnum && captureNode.PropertySnippet.Prop.Name != captureNode.UnderlyingType.Name)
            comment += $": {captureNode.UnderlyingType.Name}";

        return comment;
    }

    static string GetFullyQualifiedName(Enclosure[] enclosures) =>
        string.Join('_', enclosures.OfType<NamedEnclosure>().Select(x => x.Name));

    public override string ToString() => base.ToString();
}