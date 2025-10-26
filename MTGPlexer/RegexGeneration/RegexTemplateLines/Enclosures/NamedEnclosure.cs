using System.Diagnostics;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record NamedEnclosure : Enclosure
{
    public string Name { get; }
    public RegexPropInfo RegexPropInfo { get; }

    public NamedEnclosure
        (
            int ordinal,
            Palette palette,
            RegexPropInfo regexPropInfo, 
            string nameOverride = null
        ) : base
        (
            ordinal,
            palette,
            EnclosureType.RegexProp,
            GetTreatment(regexPropInfo)
        )
    {
        Name = nameOverride ?? regexPropInfo.Name;
        RegexPropInfo = regexPropInfo;
    }

    static GroupBorderTreatment GetTreatment(RegexPropInfo regexPropInfo) => 
        regexPropInfo.RegexPropType == RegexPropType.Enum && !regexPropInfo.IsManyOfProp
            ? GroupBorderTreatment.ClosedBox
            : GroupBorderTreatment.DashedBox;
}