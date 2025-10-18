namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Alternates;

public class AlternateValueContainer : RegexElement
{
    public List<AlternateValue> AlternateValues { get; }

    public AlternateValueContainer(Enclosure[] enclosures, List<string> alternateValueRegexes) 
        : base(enclosures, string.Join('|', alternateValueRegexes))
    {
        AlternateValues = alternateValueRegexes
            .Select((x, idx) => new AlternateValue(enclosures, x, x))
            .ToList();
    } 
}