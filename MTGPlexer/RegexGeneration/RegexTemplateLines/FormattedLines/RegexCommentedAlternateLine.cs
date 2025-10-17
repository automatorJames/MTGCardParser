namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public record RegexCommentedAlternateLine : RegexCommentedLine
{
    public object CanonicalValue { get; }
    public override string FullPath { get; }
    public CaptureGroupPropPath CaptureGroupPropPath { get; }

    public RegexCommentedAlternateLine(string regex, string comment, string enclosurePath, List<RegexCommentedLineSpan> spans, AlternateValue alternateValue)
        : base(regex, comment, enclosurePath, spans)
    {
        CanonicalValue = alternateValue.CanonicalValue;
        FullPath = EnclosurePath.Dot(CanonicalValue.ToString());
        CaptureGroupPropPath = new(enclosurePath.Dot(alternateValue.CanonicalValue.ToString()));
    }
}