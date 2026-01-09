namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record NamedEnclosure : Enclosure
{
    public string Name { get; }
    public RegexPropInfo RegexPropInfo { get; }

    public NamedEnclosure
        (
            int ordinal,
            int depth,
            RegexPropInfo regexPropInfo, 
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

    static GroupBorderTreatment GetTreatment(RegexPropInfo regexPropInfo) => 
        regexPropInfo.RegexPropType == RegexPropType.Enum && regexPropInfo.RegexPropType != RegexPropType.ManyOf
            ? GroupBorderTreatment.ClosedBox
            : GroupBorderTreatment.DashedBox;
}