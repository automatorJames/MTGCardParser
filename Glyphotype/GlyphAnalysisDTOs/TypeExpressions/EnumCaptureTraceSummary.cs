namespace Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

public class EnumCaptureTraceSummary : NamedGroupCaptureTraceSummary
{
    protected override CaptureNodeKind NodeKind => CaptureNodeKind.Enum;

    /// <summary>
    /// Occurrence count for each enum member among all CaptureTraces. Ordered by enum 
    /// member.ToString(), and includes zero-count members not represented among the CaptureTraces.
    /// </summary>
    public Dictionary<object, int> EnumMemberOccurenceCounts { get; private set; } = [];

    /// <summary>
    /// For each enum member, the occurrence count of all that member's synonyms
    /// represented among all CaptureTraces. Includes all capture string representations
    /// for the member, even if the member is represented by only one "synonym" (the usual case).
    /// </summary>
    public Dictionary<object, Dictionary<string, int>> EnumMemberSynonymOccurenceCounts { get; private set; } = [];

    /// <summary>
    /// A list of all enum members not represented among the CaptureTraces.
    /// </summary>
    public List<object> EnumMembersWithZeroOcurrences { get; private set; } = [];

    /// <summary>
    /// A list of all enum members represented by more than one synonym string among CaptureTraces.
    /// </summary>
    public List<object> EnumMembersWithMultipleSynonyms { get; private set; } = [];

    /// <summary>
    /// The count of all members in the enum type, regardless of synonyms.
    /// </summary>
    public int EnumTotalMemberCount { get; private set; }

    static readonly Dictionary<string, int> _emptySynonymOccurenceCounts = [];

    /// <summary>
    /// Occurrence count for <paramref name="memberValue"/>, or 0 if this summary has no entry for it at
    /// all — safe against a member a caller's <c>RegexGraph</c> declares but this particular summary
    /// never recorded (e.g. built against an earlier identity of a dynamically-generated enum type), where
    /// the raw <see cref="EnumMemberOccurenceCounts"/> indexer would otherwise throw.
    /// </summary>
    public int GetOccurrenceCount(object memberValue) =>
        EnumMemberOccurenceCounts.GetValueOrDefault(memberValue, 0);

    /// <summary>
    /// The synonym-pattern occurrence table for <paramref name="memberValue"/>, or an empty table if this
    /// summary has no entry for it at all. See <see cref="GetOccurrenceCount"/> for why this doesn't just
    /// index <see cref="EnumMemberSynonymOccurenceCounts"/> directly.
    /// </summary>
    public IReadOnlyDictionary<string, int> GetSynonymOccurrenceCounts(object memberValue) =>
        EnumMemberSynonymOccurenceCounts.TryGetValue(memberValue, out var counts) ? counts : _emptySynonymOccurenceCounts;

    /// <summary>
    /// Constructs a summary for an enum group that never occurred at all (its owning Glyph type
    /// had zero matches across the corpus), so every member is recorded with a zero count.
    /// </summary>
    EnumCaptureTraceSummary(string fullyQualifiedName, Type enumType)
        : base(fullyQualifiedName, [])
    {
        InitializeEnumInfo(enumType);
        var orderedEnumMembers = Enum.GetValues(enumType).Cast<object>().OrderBy(x => x.ToString());
        EnumTotalMemberCount = orderedEnumMembers.Count();
        EnumMemberOccurenceCounts = orderedEnumMembers.ToDictionary(x => x, x => 0);
        EnumMemberSynonymOccurenceCounts = orderedEnumMembers.ToDictionary(x => x, x => new Dictionary<string, int>());
    }

    public static EnumCaptureTraceSummary CreateEmpty(string fullyQualifiedName, Type enumType) => new(fullyQualifiedName, enumType);

    public EnumCaptureTraceSummary(string fullyQualifiedName, IEnumerable<Glyph> glyphs)
        : base(fullyQualifiedName, glyphs)
    {
        if (_representativeCaptureTrace == null)
            return;

        // Basic property initialization
        var enumType = _representativeCaptureTrace.ClrValue.GetType();
        InitializeEnumInfo(enumType);

        // Populate dictionaries
        foreach (var trace in CaptureTraces)
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

    void InitializeEnumInfo(Type enumType)
    {
        var orderedEnumMembers = Enum.GetValues(enumType).Cast<object>().OrderBy(x => x.ToString());
        EnumTotalMemberCount = orderedEnumMembers.Count();
        EnumMemberOccurenceCounts = orderedEnumMembers.ToDictionary(x => x, x => 0);
        EnumMemberSynonymOccurenceCounts = orderedEnumMembers.ToDictionary(x => x, x => new Dictionary<string, int>());
        EnumMembersWithZeroOcurrences = [.. orderedEnumMembers];
    }
}