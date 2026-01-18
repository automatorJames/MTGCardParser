namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class TextLine : RegexElement, IRegexContent
{
    static readonly HashSet<char> _openingPunctuationMarks = ['(', '[', '{', '<'];

    // Represents the end of a regex construct where the capture ends with a space, is enclosed in parens, and is optional
    static readonly string _endOfOptionalGroupWithTrailingSpace = " )?";
    char _plainTextLastChar;

    public string TextValue { get; set; }

    public bool ShouldOmitSpaceAfter() =>
        _openingPunctuationMarks.Contains(_plainTextLastChar)
        || TextValue.EndsWith(_endOfOptionalGroupWithTrailingSpace);

    public bool IsOptional() =>
        TextValue.EndsWith('?') // optional quantifier
        && !TextValue.EndsWith(@"\?"); // literal question mark

    public TextLine(Enclosure[] enclosures, string value, bool doNotAddPrecedingSpace = false)
        : base(
            enclosures, 
            value.Replace(" ", "[ ]"), 
            comment: $"{(value.EndsWith("?") ? "optional " : "")}literal match{(doNotAddPrecedingSpace ? " (no preceding space)" : "")}", 
            doNotAddPrecedingSpace: doNotAddPrecedingSpace)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        TextValue = value;
        _plainTextLastChar = value.Last();
    }

    public override string ToString() => base.ToString();
}