namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any TokenUnit property, but rather must be matched as part of the TokenUnit's overall Regex.
/// </summary>
public record TextSegment : RegexSegmentBase
{
    public bool DoNotAddPrecedingSpace { get; }

    public TextSegment(Snippet snippet)
    {
        var text = snippet.Text;

        if (snippet.IsOptional)
            text = $"({text} )?";

        DoNotAddPrecedingSpace = snippet.IsNoPrecedingSpace;
        RegexString = text;
    }

    public override void ComposeRegexLines(RegexBuilder builder) =>
        builder.AddTextLine(RegexString, doNotAddPrecedingSpace: DoNotAddPrecedingSpace);
}