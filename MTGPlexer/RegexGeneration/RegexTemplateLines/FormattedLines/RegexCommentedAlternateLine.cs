namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public record RegexCommentedAlternateLine : RegexCommentedLine
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public Regex MatchRegex { get; }
    public override string FullPath { get; }

    public RegexCommentedAlternateLine(string regex, string comment, string enclosurePath, int ordinal, Dictionary<int, string> colorSpans, IMatchableAlternate matchableAlt) 
        : base(regex, comment, enclosurePath, ordinal, colorSpans)
    {
        CanonicalValue = matchableAlt.CanonicalValue;
        CanonicalValueDisplay = matchableAlt.CanonicalValueDisplay;
        MatchRegex = matchableAlt.AlternateRegex;
        FullPath = EnclosurePath.Dot(CanonicalValue.ToString());
    }
}