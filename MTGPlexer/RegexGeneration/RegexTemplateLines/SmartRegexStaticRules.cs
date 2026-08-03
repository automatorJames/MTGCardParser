namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public static class SmartRegexStaticRules
{
    public static int IndentSpaces = 4;
    public static int CommentBorderLineBuffer = 2;
    public static int GroupWallInnerBuffer = 1;
    public static int GroupBookendCommentBuffer = 1;
    public static int EnumMemberOccurrenceCountColonBuffer = 1;
    public static int EnumMemberBufferAfterPipe = 1;

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

    static readonly BoxCharSet Brace = new(
        TopLeft: '\u256D', // ╭
        TopRight: '\u256E', // ╮
        BottomLeft: '\u2570', // ╰
        BottomRight: '\u256F', // ╯
        Horizontal: ' ',      //
        Vertical: '\u2506'  // ┊
    );

    public static BoxCharSet GetBoxCharsForBookendBrick(RegexBrickGroupBookend bookendBrick) =>
        NodeTypeToBoxCharSet[bookendBrick.NamedGroupParent.NodeType];

    public static Dictionary<CaptureNodeType, BoxCharSet> NodeTypeToBoxCharSet = new Dictionary<CaptureNodeType, BoxCharSet>
    {
        [CaptureNodeType.Enum] = Closed,
        [CaptureNodeType.TokenUnit] = Dashed,
        // [CaptureNodeType.QuantifierGroup] = Brace,
    };

    public static BoxCharSet ToBoxCharSet(this GroupBorderTreatment treatment) => treatment switch
    {
        GroupBorderTreatment.ClosedBox => Closed,
        GroupBorderTreatment.DashedBox => Dashed,
        GroupBorderTreatment.Brace => Brace,
        _ => Closed,
    };
}

public record BoxCharSet(char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Horizontal, char Vertical);