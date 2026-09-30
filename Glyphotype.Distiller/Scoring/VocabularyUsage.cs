namespace Glyphotype.Distiller.Scoring;

/// <summary>
/// Which vocabulary members a corpus actually used, and the texts that spelled each - what a grammar culled to the
/// corpus would keep. A vocabulary is a list of words that <em>might</em> match; once the grammar is settled, the
/// members that never matched (and synonyms that never spelled anything) can simply be dropped. So they're free:
/// <see cref="GrammarCost"/> charges only what's used here, and the corpus encoding chooses among used members only.
/// </summary>
public sealed class VocabularyUsage
{
    readonly Dictionary<string, Dictionary<string, HashSet<string>>> _spellings = [];

    public void Add(string vocabulary, string member, string spelling)
    {
        if (!_spellings.TryGetValue(vocabulary, out var members))
            _spellings[vocabulary] = members = [];

        if (!members.TryGetValue(member, out var spellings))
            members[member] = spellings = [];

        spellings.Add(spelling.Trim());
    }

    public bool IsUsed(string vocabulary, string member) =>
        _spellings.TryGetValue(vocabulary, out var members) && members.ContainsKey(member);

    /// <summary>How many of the vocabulary's members the corpus used.</summary>
    public int UsedMemberCount(string vocabulary) =>
        _spellings.TryGetValue(vocabulary, out var members) ? members.Count : 0;

    /// <summary>The texts that spelled the member in the corpus - empty if it was never used.</summary>
    public IReadOnlyCollection<string> Spellings(string vocabulary, string member) =>
        _spellings.TryGetValue(vocabulary, out var members) && members.TryGetValue(member, out var spellings) ? spellings : [];
}
