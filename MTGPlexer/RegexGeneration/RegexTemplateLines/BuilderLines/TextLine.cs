namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class TextLine : RegexElement, IRegexContent
{
    static readonly HashSet<char> _openingPunctuationMarks = ['(', '[', '{', '<'];
    static readonly HashSet<string> _escapedTerminalPunctuation = [@"\.", ",", ";", ":", "!", @"\?", @"\)", "]", "}", ">"];

    public string TextValue { get; }
    public bool EndsInOpeningPunctuation { get; }
    public bool BeginsWithTerminalPunctuation { get; }
    public bool ShouldOmitSpaceAfter { get; }
    public bool IsOptional { get; }

    public TextLine(Enclosure[] enclosures, string value)
    : base(enclosures, value.Replace(" ", "[ ]"), comment: $"{(value.EndsWith("?") ? "optional " : "")}literal match")
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        TextValue = value;
        EndsInOpeningPunctuation = _openingPunctuationMarks.Contains(value.Last());
        BeginsWithTerminalPunctuation = _escapedTerminalPunctuation.Any(value.EndsWith);

        ShouldOmitSpaceAfter = 
            EndsInOpeningPunctuation // Endings in opening punctuation should be continued without spaces
            || TextValue.EndsWith(" )?"); // Optional spaces presume the primary space is already accounted for before it

        IsOptional = TextValue.EndsWith('?') // optional quantifier
            && !TextValue.EndsWith(@"\?"); // literal question mark
    }

    public override string ToString() => base.ToString();
}