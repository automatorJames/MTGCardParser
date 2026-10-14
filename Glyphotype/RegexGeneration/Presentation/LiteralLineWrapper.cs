namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Breaks a long literal match across lines, so a phrase doesn't push every line's comment column to the right.
/// A word that would take its line, indent included, to <see cref="SmartRegexStaticRules.LiteralWrapColumn"/>
/// starts the next line instead, with the space before it leading that line - the way <c>any[ ]creatures</c> /
/// <c>[ ]with[ ]banding</c> reads. A word too long for any line keeps a line to itself; a trailing space (a joiner
/// folded onto the literal's line) stays on the last line, there being no word after it to carry.
/// <para>
/// Every space is already written <see cref="BuiltRegex.EscapedSpace"/>, never a bare space, so breaking the line
/// beside one changes nothing about the pattern for an engine reading it with whitespace ignored.
/// </para>
/// </summary>
internal static class LiteralLineWrapper
{
    /// <summary>The lines <paramref name="brick"/>'s regex text renders as - one, unless it's a literal match long enough to wrap.</summary>
    public static List<string> Wrap(RegexBrick brick)
    {
        var text = brick.RegexFormatted;

        if (brick.Parent is not TextNode || text == BuiltRegex.EscapedSpace)
            return [text];

        var indent = SmartRegexStaticRules.GetIndentSpaces(brick);
        List<string> lines = [];
        var line = "";

        foreach (var unit in SplitBeforeSpaces(text))
        {
            if (line.Length > 0 && unit != BuiltRegex.EscapedSpace && indent + line.Length + unit.Length >= SmartRegexStaticRules.LiteralWrapColumn)
            {
                lines.Add(line);
                line = "";
            }

            line += unit;
        }

        lines.Add(line);

        return lines;
    }

    /// <summary>How many lines <paramref name="brick"/> renders as - see <see cref="Wrap"/>.</summary>
    public static int LineCount(RegexBrick brick) => Wrap(brick).Count;

    /// <summary><paramref name="text"/> cut before each space: its first word, then each space with the word after it.</summary>
    static IEnumerable<string> SplitBeforeSpaces(string text)
    {
        var words = text.Split(BuiltRegex.EscapedSpace);

        yield return words[0];

        foreach (var word in words.Skip(1))
            yield return BuiltRegex.EscapedSpace + word;
    }
}
