namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record NamedEnclosure : Enclosure
{
    public string Name { get; }
    public TemplatePropInfo TemplatePropInfo { get; }

    public NamedEnclosure
        (
            int ordinal,
            int depth,
            TemplatePropInfo templatePropInfo, 
            SpaceDisposition? spaceDisposition = null
        ) : base
        (
            ordinal,
            depth,
            EnclosureType.RegexProp,
            GetTreatment(templatePropInfo),
            spaceDisposition ?? (templatePropInfo.UnderlyingType.IsDefined(typeof(NoSpacesAttribute)) ? SpaceDisposition.DisallowedLocal : SpaceDisposition.Default)
        )
    {
        Name = templatePropInfo.Name;
        TemplatePropInfo = templatePropInfo;
    }

    static GroupBorderTreatment GetTreatment(TemplatePropInfo templatePropInfo) => 
        templatePropInfo.TemplatePropType == TemplatePropType.Enum && templatePropInfo.TemplatePropType != TemplatePropType.ManyOf
            ? GroupBorderTreatment.ClosedBox
            : GroupBorderTreatment.DashedBox;
}