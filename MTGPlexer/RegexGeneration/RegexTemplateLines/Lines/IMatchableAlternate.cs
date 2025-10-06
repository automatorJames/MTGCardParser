namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public interface IMatchableAlternate
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public Regex AlternateRegex { get; }
}
