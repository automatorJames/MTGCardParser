namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Helpers;

public static class BoxChars
{
    // Unicode escape sequences for box-drawing characters.
    // This keeps the source file ASCII-safe and Git-friendly.
    private static readonly BoxCharSet Closed = new(
        TopLeft: '\u250C', // ┌
        TopRight: '\u2510', // ┐
        BottomLeft: '\u2514', // └
        BottomRight: '\u2518', // ┘
        Top: '\u2500', // ─
        Bottom: '\u2500', // ─
        Wall: '\u2502'  // │
    );

    private static readonly BoxCharSet Dashed = new(
        TopLeft: '\u250C', // ┌
        TopRight: '\u2510', // ┐
        BottomLeft: '\u2514', // └
        BottomRight: '\u2518', // ┘
        Top: '\u2500', // ─
        Bottom: '\u2500', // ─
        Wall: '\u250A'  // ┆
    );

    private static readonly BoxCharSet Brace = new(
        TopLeft: '\u256D', // ╭
        TopRight: '\u256E', // ╮
        BottomLeft: '\u2570', // ╰
        BottomRight: '\u256F', // ╯
        Top: ' ',      //
        Bottom: ' ',      //
        Wall: '\u2506'  // ┊
    );

    public static BoxCharSet Get(GroupBorderTreatment treatment) => treatment switch
    {
        GroupBorderTreatment.ClosedBox => Closed,
        GroupBorderTreatment.DashedBox => Dashed,
        GroupBorderTreatment.Brace => Brace,
        _ => Closed,
    };
}
