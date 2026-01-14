namespace MTGPlexer.RegexGeneration.RegexTemplateLines.FormattedLines;

/// <summary>
/// A centralized record to hold all the coloring rules for the generated regex.
/// It references the base color consts above for easy tweaking.
/// </summary>
record FormattedRegexColoringRules
{
    public const string Black = "#000000"; // 0% white
    public const string Grey10 = "#1A1A1A"; // 10% white
    public const string Grey20 = "#333333"; // 20% white
    public const string Grey30 = "#4D4D4D"; // 30% white
    public const string Grey40 = "#666666"; // 40% white
    public const string Grey50 = "#808080"; // 50% white (true mid-grey)
    public const string Grey60 = "#999999"; // 60% white
    public const string Grey70 = "#B3B3B3"; // 70% white
    public const string Grey80 = "#CCCCCC"; // 80% white
    public const string Grey90 = "#E6E6E6"; // 90% white (almost white)
    public const string White = "#FFFFFF"; // 100% white

    // General Element Coloring Rules
    // Note: DefaultRegexTextColor is now mostly a fallback, as primary content color is dynamically picked.
    public string DefaultRegexTextColor { get; } = Grey80;
    public string HashSeparatorColor { get; } = Grey20;
    public string UnenclosedTextLineCommentColor { get; } = White;
    public string UnenclosedSpaceLineCommentColor { get; } = Grey50;
    public string BoundaryCommentColor { get; } = Grey30;
    public string GroupCloseQuantifierColor { get; } = Grey40;
    public string DefaultFallbackColor { get; } = Grey30;
    public string OmittedEnumCountColor { get; } = Grey30;

    // Palette-Dependent Coloring Rules
    public Func<HexPalette, string> AlternateValueCommentColor { get; } = p => p.Light;
    public Func<HexPalette, string> SynonymValueCommentColor { get; } = p => p.Dark;
    public Func<HexPalette, string> NamedGroupBookendCommentColor { get; } = p => p.Sat;
    public Func<HexPalette, string> EnclosedTextColor { get; } = p => p.Normal;

    // Border Coloring Rules based on Treatment
    private Func<HexPalette, string> ClosedBoxBorderColor { get; } = p => p.Normal;
    private Func<HexPalette, string> DashedBoxBorderColor { get; } = p => p.Dark;
    private string BraceBorderColor { get; } = Grey60;

    public string GetBorderColor(GroupBorderTreatment treatment, HexPalette palette) => treatment switch
    {
        GroupBorderTreatment.ClosedBox => ClosedBoxBorderColor(palette),
        GroupBorderTreatment.DashedBox => DashedBoxBorderColor(palette),
        GroupBorderTreatment.Brace => BraceBorderColor,
        _ => DefaultFallbackColor // Default for unknown treatment
    };
}
