namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValueContainer : RegexElement
{
    public List<AlternateValue> AlternateValues { get; }

    public AlternateValueContainer(Enclosure[] enclosures, List<string> alternateValueRegexes) 
        : base(enclosures, string.Join('|', alternateValueRegexes))
    {
        AlternateValues = alternateValueRegexes
            .Select((x, idx) => new AlternateValue(enclosures, x, x, idx == 0))
            .ToList();
    } 

    public AlternateValueContainer(Enclosure[] enclosures, EnumScalarAlternateSet enumScalarAlternateSet) 
        : base(enclosures, string.Join('|', enumScalarAlternateSet.EnumAlternates.Select(x => x.RegexString)))
    {
        AlternateValues = enumScalarAlternateSet.EnumAlternates
            .Select((x, idx) => new AlternateValue(enclosures, x.RegexString, x.EnumValue.ToString(), idx == 0))
            .ToList();
    }
}