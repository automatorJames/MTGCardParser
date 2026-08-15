namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Column-alignment measurements for one dynamic group's resolved-type rows: the widest resolved type
/// name and occurrence-count digit string among the rows actually being shown, so every row's colon
/// lines up in the same column regardless of that row's own name/count length. Mirrors <see cref="EnumColumnMetrics"/>.
/// </summary>
internal readonly struct DynamicColumnMetrics
{
    /// <summary>Length of the longest resolved type name being displayed.</summary>
    public int MaxNameLength { get; }

    /// <summary>Length of the longest occurrence-count digit string being displayed.</summary>
    public int MaxDigitLength { get; }

    /// <summary>The widest comment any row in this dynamic group's section needs.</summary>
    public int MaxCommentLength { get; }

    static string ColonBuffer => string.Empty.PadLeft(SmartRegexStaticRules.EnumMemberOccurrenceCountColonBuffer);

    DynamicColumnMetrics(int maxNameLength, int maxDigitLength, int maxCommentLength)
    {
        MaxNameLength = maxNameLength;
        MaxDigitLength = maxDigitLength;
        MaxCommentLength = maxCommentLength;
    }

    /// <summary>Measures the columns needed for every resolved type and its per-capture-value counts.</summary>
    public static DynamicColumnMetrics Calculate(Dictionary<Type, Dictionary<string, int>> resolvedTypeCaptureValueOccurrenceCounts)
    {
        int maxNameLength = 0;
        int maxDigitLength = 0;

        foreach (var (type, captureValueCounts) in resolvedTypeCaptureValueOccurrenceCounts)
        {
            maxNameLength = Math.Max(maxNameLength, type.Name.Length);
            maxDigitLength = Math.Max(maxDigitLength, captureValueCounts.Values.Sum().ToString().Length);

            foreach (var count in captureValueCounts.Values)
                maxDigitLength = Math.Max(maxDigitLength, count.ToString().Length);
        }

        var colonBufferLength = SmartRegexStaticRules.EnumMemberOccurrenceCountColonBuffer;
        var maxCommentLength = maxNameLength + colonBufferLength + 1 + colonBufferLength + maxDigitLength;

        return new(maxNameLength, maxDigitLength, maxCommentLength);
    }

    /// <summary>The name field alone, right-aligned within <see cref="MaxNameLength"/>.</summary>
    public string FormatNameField(string typeName) =>
        typeName.PadLeft(MaxNameLength);

    /// <summary>A blank name field, still reserving <see cref="MaxNameLength"/> columns, for a per-value row grouped under a header.</summary>
    public string FormatBlankNameField() =>
        string.Empty.PadLeft(MaxNameLength);

    /// <summary>The " : count" field alone (including its leading colon buffer), left-aligned within <see cref="MaxDigitLength"/>.</summary>
    public string FormatCountField(int occurrenceCount) =>
        $"{ColonBuffer}:{ColonBuffer}{occurrenceCount.ToString().PadRight(MaxDigitLength)}";
}
