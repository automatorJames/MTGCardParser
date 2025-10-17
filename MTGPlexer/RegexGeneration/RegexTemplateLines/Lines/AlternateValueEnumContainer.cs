namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValueEnumContainer : AlternateValueContainer
{
    public EnumScalarAlternateSet EnumScalarAlternateSet { get; }
    public List<AlternateValueEnum> AlternateValueEnums { get; }

    public AlternateValueEnumContainer(Enclosure[] enclosures, EnumScalarAlternateSet enumScalarAlternateSet)
        : base(enclosures, enumScalarAlternateSet.Alternates)
    {
        EnumScalarAlternateSet = enumScalarAlternateSet;
        AlternateValueEnums = enumScalarAlternateSet.EnumAlternates.Select(x => new AlternateValueEnum(enclosures, x)).ToList();
    }
}