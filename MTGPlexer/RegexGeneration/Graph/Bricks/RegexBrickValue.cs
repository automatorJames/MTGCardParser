namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>A brick that matches one specific scalar value, e.g. a single enum member's regex pattern.</summary>
public class RegexBrickValue : RegexBrick
{
    /// <summary>The CLR value (e.g. enum member) this brick's pattern matches.</summary>
    public object Value { get; }

    /// <summary>Display-only: the leading pipe/space (plus buffer) that joins this row's regex text to the row above, split out of <see cref="RegexBrick.RegexFormatted"/> so it can be colored separately. Populated by <see cref="EnumSectionBuilder"/>.</summary>
    public string JoinerRegexFormatted { get; set; } = "";

    /// <summary>Display-only: this row's regex pattern text alone, without the leading joiner. Populated by <see cref="EnumSectionBuilder"/>.</summary>
    public string MemberRegexFormatted { get; set; } = "";

    /// <summary>Display-only: the member-name field of this row's comment (blank, but width-reserved, for a synonym row). Populated by <see cref="EnumSectionBuilder"/>.</summary>
    public string NameCommentFormatted { get; set; } = "";

    /// <summary>Display-only: the ": count" field of this row's comment. Populated by <see cref="EnumSectionBuilder"/>.</summary>
    public string CountCommentFormatted { get; set; } = "";

    public RegexBrickValue(RegexNode parentNode, string regex, object value)
        : base(parentNode, regex)
    {
        Value = value;
    }
}