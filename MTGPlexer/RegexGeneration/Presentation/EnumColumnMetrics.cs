namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Column-alignment measurements for one enum group's member comment rows: the widest member name and
/// occurrence-count digit string among the members actually being shown, so every row's colon lines up
/// in the same column regardless of that row's own name/count length.
/// </summary>
internal readonly struct EnumColumnMetrics
{
    /// <summary>Length of the longest member name being displayed.</summary>
    public int MaxNameLength { get; }

    /// <summary>Length of the longest occurrence-count digit string being displayed.</summary>
    public int MaxDigitLength { get; }

    /// <summary>The widest comment any row in this enum's section needs, including the omitted-count row if present.</summary>
    public int MaxCommentLength { get; }

    static string ColonBuffer => string.Empty.PadLeft(SmartRegexStaticRules.EnumMemberOccurrenceCountColonBuffer);

    EnumColumnMetrics(int maxNameLength, int maxDigitLength, int maxCommentLength)
    {
        MaxNameLength = maxNameLength;
        MaxDigitLength = maxDigitLength;
        MaxCommentLength = maxCommentLength;
    }

    /// <summary>Measures the columns needed for <paramref name="members"/>, accounting for the omitted-count brick's comment if one was built.</summary>
    public static EnumColumnMetrics Calculate(List<RegexBrickValue> members, EnumCaptureTraceSummary enumSummary, RegexBrickOmittedCount omittedCount)
    {
        var maxNameLength = members.Max(x => x.Value.ToString().Length);
        var maxDigitLength = members.Max(x => enumSummary.EnumMemberOccurenceCounts[x.Value].ToString().Length);
        var colonBufferLength = SmartRegexStaticRules.EnumMemberOccurrenceCountColonBuffer;
        var coreLength = maxNameLength + colonBufferLength + 1 + colonBufferLength + maxDigitLength;
        var maxCommentLength = Math.Max(coreLength, omittedCount?.CommentFormatted.Length ?? 0);

        return new(maxNameLength, maxDigitLength, maxCommentLength);
    }

    /// <summary>"Name : count", with the name right-aligned and the count left-aligned so every row's colon lines up.</summary>
    public string FormatNamedCore(object memberValue, int occurrenceCount) =>
        $"{memberValue.ToString().PadLeft(MaxNameLength)}{ColonBuffer}:{ColonBuffer}{occurrenceCount.ToString().PadRight(MaxDigitLength)}";

    /// <summary>"     : count", with the name column blanked but still reserved, for a synonym row grouped under a header.</summary>
    public string FormatMinimalCore(int synonymOccurrenceCount) =>
        $"{string.Empty.PadLeft(MaxNameLength)}{ColonBuffer}:{ColonBuffer}{synonymOccurrenceCount.ToString().PadRight(MaxDigitLength)}";
}
