namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Alternates;

public class AlternateValueContainer : RegexElement, IRegexContent
{
    public List<AlternateValue> AlternateValues { get; }
    public string TextValue { get; set; }

    public AlternateValueContainer(Enclosure[] enclosures, List<string> alternateValueRegexes) 
        : base(enclosures, string.Join('|', alternateValueRegexes))
    {
        AlternateValues = alternateValueRegexes
            .Select((x, idx) => new AlternateValue(enclosures, x, x))
            .ToList();

        TextValue = string.Join('|', AlternateValues.Select(x => x.CanonicalValue));
    } 
}