
namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

public record RegexCommentedLine
{
    Dictionary<int, string> _colorSpans;

    public string Regex { get; }
    public string Comment { get; }
    public string EnclosurePath { get; }
    public virtual string FullPath { get; }
    public int Ordinal { get; }
    public string FormattedText { get; }

    public RegexCommentedLine(string regex, string comment, string enclosurePath, int ordinal, Dictionary<int, string> colorSpans)
    {
        Regex = regex;
        Comment = comment;
        EnclosurePath = enclosurePath;
        FullPath = enclosurePath;
        Ordinal = ordinal;
        FormattedText = Regex + Comment;
        _colorSpans = colorSpans;
    }

    public Dictionary<int, string> GetColorSpans() => _colorSpans;

}