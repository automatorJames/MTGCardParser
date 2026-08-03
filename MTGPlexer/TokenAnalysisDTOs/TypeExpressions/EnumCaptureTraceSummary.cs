namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public class EnumCaptureTraceSummary
{
    /// <summary>
    /// The fully qualified name that represents the path from the RegexGraph root node
    /// to the enum property being summarized herein.
    /// </summary>
    public string FullyQualifiedName { get; }

    /// <summary>
    /// Occurrence count for each enum member among all CaptureTraces. Ordered by enum 
    /// member.ToString(), and includes zero-count members not represented among the CaptureTraces.
    /// </summary>
    public Dictionary<object, int> EnumMemberOccurenceCounts { get; } = [];

    /// <summary>
    /// For each enum member, the occurrence count of all that member's synonyms
    /// represented among all CaptureTraces. Includes all capture string representations
    /// for the member, even if the member is represented by only one "synonym" (the usual case).
    /// </summary>
    public Dictionary<object, Dictionary<string, int>> EnumMemberSynonymOccurenceCounts { get; } = [];

    /// <summary>
    /// A list of all enum members not represented among the CaptureTraces.
    /// </summary>
    public List<object> EnumMembersWithZeroOcurrences { get; } = [];

    /// <summary>
    /// A list of all enum members represented by more than one synonym string among CaptureTraces.
    /// </summary>
    public List<object> EnumMembersWithMultipleSynonyms { get; } = [];

    /// <summary>
    /// The count of all members in the enum type, regardless of synonyms.
    /// </summary>
    public int EnumTotalMemberCount { get; }

    public EnumCaptureTraceSummary(IEnumerable<CaptureTrace> enumCaptureTracesOfQualifiedName)
    {
        // Validity checks
        if (enumCaptureTracesOfQualifiedName == null || !enumCaptureTracesOfQualifiedName.Any())
            throw new Exception($"Cannot construct {nameof(EnumCaptureTraceSummary)} with null or empty collection");

        var distinctNames = enumCaptureTracesOfQualifiedName.Select(x => x.FullyQualifiedName).Distinct();
        
        if (distinctNames.Count() > 1)
            throw new Exception($"Expected exactly one distinct {nameof(CaptureTrace.FullyQualifiedName)}, but got {distinctNames.Count()}");

        var representativeTrace = enumCaptureTracesOfQualifiedName.First();
        
        if (representativeTrace.NodeType != CaptureNodeType.Enum)
            throw new Exception($"Expected {nameof(CaptureTrace.NodeType)} {CaptureNodeType.Enum}, but got {representativeTrace.NodeType}");

        // Basic property initialization
        FullyQualifiedName = representativeTrace.FullyQualifiedName;
        var enumType = representativeTrace.ClrValue.GetType();
        var orderedEnumMembers = Enum.GetValues(enumType).Cast<object>().OrderBy(x => x.ToString());
        EnumTotalMemberCount = orderedEnumMembers.Count();
        EnumMemberOccurenceCounts = orderedEnumMembers.ToDictionary(x => x, x => 0);
        EnumMemberSynonymOccurenceCounts = orderedEnumMembers.ToDictionary(x => x, x => new Dictionary<string, int>());

        // Populate dictionaries
        foreach (var trace in enumCaptureTracesOfQualifiedName)
        {
            EnumMemberOccurenceCounts[trace.ClrValue]++;
            EnumMemberSynonymOccurenceCounts[trace.ClrValue].TryAdd(trace.CaptureValue, 0);
            EnumMemberSynonymOccurenceCounts[trace.ClrValue][trace.CaptureValue]++;
        }

        // Derived property initialization
        EnumMembersWithZeroOcurrences = EnumMemberOccurenceCounts
            .Where(x => x.Value == 0)
            .Select(x => x.Key)
            .ToList();

        EnumMembersWithMultipleSynonyms = EnumMemberSynonymOccurenceCounts
            .Where(x => x.Value.Count > 1)
            .Select(x => x.Key)
            .ToList();
    }

    public static Dictionary<string, EnumCaptureTraceSummary> GetFromCaptureTraces(IEnumerable<CaptureTrace> enumTerminalCaptureTraces)
    {
        Dictionary<string, EnumCaptureTraceSummary> dict = [];
        var groupedTraces = enumTerminalCaptureTraces.GroupBy(x => x.FullyQualifiedName);

        foreach (var group in groupedTraces)
            dict[group.Key] = new(group);

        return dict;
    }

    public bool MemberIsFirstAmongManyRepresentedSynonyms(object memberValue, string synonym)
    {
        if (!EnumMembersWithMultipleSynonyms.Contains(memberValue))
            return false;

        var synonymKeys = EnumMemberSynonymOccurenceCounts[memberValue].Keys.ToList();

        return synonymKeys.IndexOf(synonym) == 0;
    }
}