namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Central, easily-tweakable knobs for the formatted/commented regex output: spacing, buffer widths,
/// and the box-drawing character sets used to render group borders. Nothing outside this class should
/// hardcode a layout constant for formatted output.
/// </summary>
public static class SmartRegexStaticRules
{
    /// <summary>Spaces of indentation per level of group nesting in the regex column.</summary>
    public static int IndentSpaces = 4;

    /// <summary>Extra columns of padding before the '#' comment separator.</summary>
    public static int CommentBorderLineBuffer = 2;

    /// <summary>Spaces between a group box's wall character and its contents.</summary>
    public static int GroupWallInnerBuffer = 1;

    /// <summary>Spaces between a group bookend's box corner and its comment text.</summary>
    public static int GroupBookendCommentBuffer = 1;

    /// <summary>Spaces on either side of the colon separating an enum member's name from its occurrence count.</summary>
    public static int EnumMemberOccurrenceCountColonBuffer = 1;

    /// <summary>Spaces between an enum member row's leading pipe/space and its regex pattern.</summary>
    public static int EnumMemberBufferAfterPipe = 1;

    /// <summary>The literal "  #  "-style prefix that separates a line's regex column from its comment column.</summary>
    public static string CommentBorderLineWithBuffer =
        $"{string.Empty.PadLeft(CommentBorderLineBuffer)}#{string.Empty.PadLeft(CommentBorderLineBuffer)}";

    /// <summary>
    /// Centers text within the given width. Any odd leftover space goes to the right,
    /// so repeated centering (e.g. centering an already-centered string within a wider
    /// outer width) stays visually balanced.
    /// </summary>
    public static string CenterPad(string text, int width)
    {
        var extra = Math.Max(0, width - text.Length);
        var leftPad = extra / 2;
        var rightPad = extra - leftPad;
        return $"{string.Empty.PadLeft(leftPad)}{text}{string.Empty.PadLeft(rightPad)}";
    }

    // Unicode escape sequences for box-drawing characters.
    // This keeps the source file ASCII-safe and Git-friendly.
    static readonly BoxCharSet Closed = new(
        TopLeft: '\u250C', // ┌
        TopRight: '\u2510', // ┐
        BottomLeft: '\u2514', // └
        BottomRight: '\u2518', // ┘
        Horizontal: '\u2500', // ─
        Vertical: '\u2502'  // │
    );

    static readonly BoxCharSet Dashed = new(
        TopLeft: '\u250C', // ┌
        TopRight: '\u2510', // ┐
        BottomLeft: '\u2514', // └
        BottomRight: '\u2518', // ┘
        Horizontal: '\u2500', // ─
        Vertical: '\u250A'  // ┆
    );

    /// <summary>Which <see cref="BoxCharSet"/> to draw around a named group's border, keyed by the kind of node it represents.</summary>
    public static Dictionary<CaptureNodeType, BoxCharSet> NodeTypeToBoxCharSet = new Dictionary<CaptureNodeType, BoxCharSet>
    {
        [CaptureNodeType.Enum] = Closed,
        [CaptureNodeType.TokenUnit] = Dashed,
    };

    /// <summary>Looks up the box-drawing character set for the group a bookend brick opens or closes.</summary>
    public static BoxCharSet GetBoxCharsForBookendBrick(RegexBrickGroupBookend bookendBrick) =>
        NodeTypeToBoxCharSet[bookendBrick.NamedGroupParent.NodeType];
}

public record BoxCharSet(char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Horizontal, char Vertical);