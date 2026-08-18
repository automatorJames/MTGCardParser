namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Central, easily-tweakable knobs for the formatted/commented regex output: spacing, buffer widths,
/// and the box-drawing character sets used to render group borders. Nothing outside this class should
/// hardcode a layout constant for formatted output.
/// </summary>
public static class SmartRegexStaticRules
{
    /// <summary>Spaces of indentation per level of group nesting in the regex column.</summary>
    public static int IndentSpaces = 4;

    /// <summary>
    /// Spaces of indentation for an enum member row (and its synonym header/footer/omitted-count siblings)
    /// from its enclosing enum group's own bookends — narrower than <see cref="IndentSpaces"/> since these
    /// rows don't themselves introduce another level of group nesting.
    /// </summary>
    public static int EnumMemberIndentSpaces = 2;

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
        var (leftPad, rightPad) = CenterPadSplit(text, width);
        return $"{leftPad}{text}{rightPad}";
    }

    /// <summary>
    /// Same padding math as <see cref="CenterPad"/>, but returns the two pad strings separately instead of
    /// a merged result — for callers that need to attach the padding to sub-pieces of <paramref name="text"/>
    /// (e.g. coloring a centered comment's name/count fields differently) rather than to the whole string.
    /// </summary>
    public static (string LeftPad, string RightPad) CenterPadSplit(string text, int width)
    {
        var extra = Math.Max(0, width - text.Length);
        var leftPad = extra / 2;
        var rightPad = extra - leftPad;
        return (string.Empty.PadLeft(leftPad), string.Empty.PadLeft(rightPad));
    }

    // Box-drawing characters
    const char BoxTopLeft = '\u250C';        // ┌
    const char BoxTopRight = '\u2510';       // ┐
    const char BoxBottomLeft = '\u2514';     // └
    const char BoxBottomRight = '\u2518';    // ┘
    const char BoxHorizontal = '\u2500';     // ─
    const char BoxVertical = '\u2502';       // │
    const char BoxVerticalDashed = '\u250A'; // ┆

    // Ordinary / distinguishing characters
    const char Asterisk = '\u002A';          // *
    const char Plus = '\u002B';              // +
    const char Hyphen = '\u002D';            // -
    const char Hash = '\u0023';              // #
    const char Colon = '\u003A';             // :
    const char Period = '\u002E';            // .
    const char MiddleDot = '\u00B7';         // ·
    const char Bullet = '\u2022';            // •
    const char WhiteCircle = '\u25CB';       // ○
    const char BlackCircle = '\u25CF';       // ●
    const char WhiteDiamond = '\u25C7';      // ◇
    const char BlackDiamond = '\u25C6';      // ◆
    const char WhiteSquare = '\u25A1';       // □
    const char BlackSquare = '\u25A0';       // ■
    const char RightArrow = '\u2192';        // →
    const char LeftArrow = '\u2190';         // ←
    
    static readonly BoxCharSet Closed = new(
        TopLeft: BoxTopLeft,
        TopRight: BoxTopRight,
        BottomLeft: BoxBottomLeft,
        BottomRight: BoxBottomRight,
        Horizontal: BoxHorizontal,
        Vertical: BoxVertical
    );

    static readonly BoxCharSet Dashed = Closed with { Vertical = BoxVerticalDashed };

    static readonly BoxCharSet AllBullets = new(
        TopLeft: Bullet,
        TopRight: Bullet,
        BottomLeft: Bullet,
        BottomRight: Bullet,
        Horizontal: Bullet,
        Vertical: Bullet
    );

    /// <summary>Which <see cref="BoxCharSet"/> to draw around a named group's border, keyed by the kind of node it represents.</summary>
    public static Dictionary<CaptureNodeKind, BoxCharSet> NodeTypeToBoxCharSet = new Dictionary<CaptureNodeKind, BoxCharSet>
    {
        [CaptureNodeKind.Enum] = Closed,
        [CaptureNodeKind.Token] = Dashed,
        [CaptureNodeKind.OneOf] = Dashed,
        [CaptureNodeKind.Dynamic] = AllBullets,
        [CaptureNodeKind.Int] = Dashed,
        [CaptureNodeKind.Bool] = Dashed,
    };

    /// <summary>Looks up the box-drawing character set for the group a bookend brick opens or closes.</summary>
    public static BoxCharSet GetBoxCharsForBookendBrick(RegexBrickGroupBookend bookendBrick) =>
        NodeTypeToBoxCharSet[bookendBrick.NamedGroupParent.NodeKind];

    /// <summary>
    /// The regex-column indent for <paramref name="brick"/>: <see cref="EnumMemberIndentSpaces"/> for its
    /// last level of depth when it's a row directly inside an enum group (a member, synonym header/footer,
    /// or omitted-count row — not the group's own bookends, which stay at the ordinary per-level indent),
    /// <see cref="IndentSpaces"/> per level otherwise.
    /// </summary>
    public static int GetIndentSpaces(RegexBrick brick) =>
        brick is not RegexBrickGroupBookend && brick.NamedGroupParent is EnumNode
            ? ((brick.NestedDepth - 1) * IndentSpaces) + EnumMemberIndentSpaces
            : brick.NestedDepth * IndentSpaces;
}

public record BoxCharSet(char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Horizontal, char Vertical);