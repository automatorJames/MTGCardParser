namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public record RegexCommentedAlternateLine : RegexCommentedLine
{
    public object CanonicalValue { get; }
    public string CanonicalValueDisplay { get; }
    public Regex MatchRegex { get; }
    public override string FullPath { get; }
    public CaptureGroupPropPath CaptureGroupPropPath { get; }

    public RegexCommentedAlternateLine(string regex, string comment, string enclosurePath, int ordinal, List<RegexCommentedLineSpan> spans, IMatchableAlternate matchableAlt)
        : base(regex, comment, enclosurePath, ordinal, spans)
    {
        CanonicalValue = matchableAlt.CanonicalValue;
        CanonicalValueDisplay = matchableAlt.CanonicalValueDisplay;
        MatchRegex = matchableAlt.AlternateRegex;
        FullPath = EnclosurePath.Dot(CanonicalValue.ToString());
        CaptureGroupPropPath = new(enclosurePath.Dot(matchableAlt.CanonicalValue.ToString()));
    }
}