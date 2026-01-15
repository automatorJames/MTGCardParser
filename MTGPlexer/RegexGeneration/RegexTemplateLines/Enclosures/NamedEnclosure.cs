namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record NamedEnclosure : Enclosure
{
    public string Name { get; }
    public TemplatePropInfo RegexPropInfo { get; }

    public NamedEnclosure
        (
            int ordinal,
            int depth,
            TemplatePropInfo regexPropInfo, 
            SpaceDisposition? spaceDisposition = null
        ) : base
        (
            ordinal,
            depth,
            EnclosureType.RegexProp,
            GetTreatment(regexPropInfo),
            spaceDisposition ?? (regexPropInfo.BaseType.IsDefined(typeof(NoSpacesAttribute)) ? SpaceDisposition.DisallowedLocal : SpaceDisposition.Default)
        )
    {
        Name = regexPropInfo.Name;
        RegexPropInfo = regexPropInfo;
    }

    static GroupBorderTreatment GetTreatment(TemplatePropInfo regexPropInfo) => 
        regexPropInfo.TemplatePropType == RegexPropType.Enum && regexPropInfo.TemplatePropType != RegexPropType.ManyOf
            ? GroupBorderTreatment.ClosedBox
            : GroupBorderTreatment.DashedBox;
}