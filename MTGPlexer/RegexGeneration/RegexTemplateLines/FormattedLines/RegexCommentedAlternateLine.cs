namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public record RegexCommentedAlternateLine : RegexCommentedLine
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public int Ordinal { get; }
    public Regex MatchRegex { get; }
    public override string FullPath { get; }
    public CaptureGroupPropPath CaptureGroupPropPath { get; }

    public RegexCommentedAlternateLine(string regex, string comment, string enclosurePath, List<RegexCommentedLineSpan> spans, IMatchableAlternate matchableAlt)
        : base(regex, comment, enclosurePath, spans)
    {
        CanonicalValue = matchableAlt.CanonicalValue;
        CanonicalValueDisplay = matchableAlt.CanonicalValueDisplay;
        Ordinal = matchableAlt.Ordinal;
        MatchRegex = matchableAlt.AlternateRegex;
        FullPath = EnclosurePath.Dot(CanonicalValue.ToString());
        CaptureGroupPropPath = new(enclosurePath.Dot(matchableAlt.CanonicalValue.ToString()));
    }
}