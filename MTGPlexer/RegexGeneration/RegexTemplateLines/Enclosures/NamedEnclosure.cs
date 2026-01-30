using MTGPlexer.RegexGeneration.GraphNodes;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record NamedEnclosure : Enclosure
{
    public string Name { get; }
    public CaptureNode CaptureNode { get; }

    public NamedEnclosure
        (
            int ordinal,
            int depth,
            CaptureNode captureNode, 
            SpaceDisposition? spaceDisposition = null
        ) : base
        (
            ordinal,
            depth,
            EnclosureType.RegexProp,
            GetTreatment(captureNode),
            spaceDisposition ?? (captureNode.UnderlyingType.IsDefined(typeof(NoSpacesAttribute)) ? SpaceDisposition.DisallowedLocal : SpaceDisposition.Default)
        )
    {
        Name = captureNode.Name;
        CaptureNode = captureNode;
    }

    static GroupBorderTreatment GetTreatment(CaptureNode captureNode) =>
        captureNode.UnderlyingType switch
        {
            { IsEnum: true } => GroupBorderTreatment.ClosedBox,
            _ => GroupBorderTreatment.DashedBox
        };
}