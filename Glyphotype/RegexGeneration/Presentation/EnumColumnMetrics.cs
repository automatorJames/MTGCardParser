namespace Glyphotype.RegexGeneration.Presentation;

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
        int maxNameLength = 0;
        int maxDigitLength = 0;

        if (members.Any())
        {
            maxNameLength = members.Max(x => x.Value.ToString().Length);
            maxDigitLength = members.Max(x => enumSummary.GetOccurrenceCount(x.Value).ToString().Length);
        }

        var colonBufferLength = SmartRegexStaticRules.EnumMemberOccurrenceCountColonBuffer;
        var coreLength = maxNameLength + colonBufferLength + 1 + colonBufferLength + maxDigitLength;
        var maxCommentLength = Math.Max(coreLength, omittedCount?.CommentFormatted.Length ?? 0);

        return new(maxNameLength, maxDigitLength, maxCommentLength);
    }

    /// <summary>"     : count", with the name column blanked but still reserved, for a synonym row grouped under a header.</summary>
    public string FormatMinimalCore(int synonymOccurrenceCount) =>
        FormatBlankNameField() + FormatCountField(synonymOccurrenceCount);

    /// <summary>The name field alone, right-aligned within <see cref="MaxNameLength"/>.</summary>
    public string FormatNameField(object memberValue) =>
        memberValue.ToString().PadLeft(MaxNameLength);

    /// <summary>A blank name field, still reserving <see cref="MaxNameLength"/> columns, for a synonym row grouped under a header.</summary>
    public string FormatBlankNameField() =>
        string.Empty.PadLeft(MaxNameLength);

    /// <summary>The " : count" field alone (including its leading colon buffer), left-aligned within <see cref="MaxDigitLength"/>.</summary>
    public string FormatCountField(int occurrenceCount) =>
        $"{ColonBuffer}:{ColonBuffer}{occurrenceCount.ToString().PadRight(MaxDigitLength)}";
}
