using System.Security.Cryptography;
using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Distiller.Workspaces;

/// <summary>What a <see cref="JournalEntry"/> records.</summary>
public enum JournalSection
{
    /// <summary>Something the grammar doesn't handle yet, and what's known about it.</summary>
    OpenProblem,

    /// <summary>An approach that was tried and didn't pay off, and why - so it isn't tried again blind.</summary>
    DeadEnd,

    /// <summary>Anything else worth knowing before doing the work: a quirk of the corpus, a trap, a lead.</summary>
    Hint,
}

/// <summary>
/// One finding in a <see cref="WorkspaceJournal"/>.
/// </summary>
/// <param name="Names">The glyphs and vocabularies the entry is about.</param>
/// <param name="Fingerprints">
/// How each of <paramref name="Names"/> read in the working grammar when the entry was last written (empty for one it
/// didn't have) - so an entry whose glyphs have changed since can be marked as maybe out of date.
/// </param>
public sealed record JournalEntry(int Id, JournalSection Section, string Text, IReadOnlyList<string> Names, IReadOnlyDictionary<string, string> Fingerprints, DateTimeOffset Updated)
{
    public int Words => WorkspaceJournal.CountWords(Text);
}

/// <summary>A copy of a journal as it stood when its workspace was committed or checkpointed.</summary>
public sealed record JournalVersion(string Id, string Label, DateTimeOffset At);

/// <summary>
/// A workspace's journal: what agents working on its grammar have learned that the next one would otherwise have to
/// learn again - open problems, dead ends and hints, not a log of what was done (the step history is that). Every
/// session reads it whole, and keeps it current: adding what it finds, and removing what no longer holds.
/// <para>
/// Kept in the workspace's folder apart from the grammar, so no step, undo or revert touches it - what an undone step
/// taught stays learned. A copy is kept each time the workspace is committed or checkpointed (see <see cref="Snapshot"/>),
/// for looking back at what was known then.
/// </para>
/// </summary>
public sealed class WorkspaceJournal
{
    const string _fileName = "journal.json";
    const string _versionsFolderName = "journal-versions";

    readonly string _folder;
    readonly object _gate = new();

    sealed record Saved(int NextId, List<JournalEntry> Entries);
    sealed record SavedVersion(string Label, DateTimeOffset At, List<JournalEntry> Entries);

    public WorkspaceJournal(string folder) => _folder = folder;

    /// <summary>Fires, from whatever thread, whenever the journal changes.</summary>
    public event Action Changed;

    string FilePath => Path.Combine(_folder, _fileName);

    string VersionsFolder => Path.Combine(_folder, _versionsFolderName);

    /// <summary>The entries, by section and then as they were added.</summary>
    public IReadOnlyList<JournalEntry> Entries
    {
        get
        {
            lock (_gate)
                return Order(Load().Entries);
        }
    }

    /// <summary>How many words the entries come to.</summary>
    public int Words => Entries.Sum(x => x.Words);

    /// <summary>Adds an entry about <paramref name="names"/>, fingerprinting them as <paramref name="working"/> has them.</summary>
    public JournalEntry Add(JournalSection section, string text, IEnumerable<string> names, GrammarDefinition working)
    {
        JournalEntry entry;

        lock (_gate)
        {
            var saved = Load();
            var nameList = CleanNames(names);
            entry = new(saved.NextId, section, RequireText(text), nameList, Fingerprint(working, nameList), DateTimeOffset.Now);

            saved.Entries.Add(entry);
            Save(saved with { NextId = saved.NextId + 1 });
        }

        Changed?.Invoke();
        return entry;
    }

    /// <summary>Rewrites entry <paramref name="id"/> - whatever's given of its section, text and names - and fingerprints its names afresh, as <paramref name="working"/> has them.</summary>
    public JournalEntry Update(int id, JournalSection? section, string text, IEnumerable<string> names, GrammarDefinition working)
    {
        JournalEntry entry;

        lock (_gate)
        {
            var saved = Load();
            var index = IndexOf(saved, id);
            var old = saved.Entries[index];
            var nameList = names is null ? old.Names : CleanNames(names);

            entry = old with
            {
                Section = section ?? old.Section,
                Text = string.IsNullOrWhiteSpace(text) ? old.Text : text.Trim(),
                Names = nameList,
                Fingerprints = Fingerprint(working, nameList),
                Updated = DateTimeOffset.Now,
            };

            saved.Entries[index] = entry;
            Save(saved);
        }

        Changed?.Invoke();
        return entry;
    }

    public JournalEntry Remove(int id)
    {
        JournalEntry entry;

        lock (_gate)
        {
            var saved = Load();
            var index = IndexOf(saved, id);
            entry = saved.Entries[index];

            saved.Entries.RemoveAt(index);
            Save(saved);
        }

        Changed?.Invoke();
        return entry;
    }

    /// <summary>Whether any of <paramref name="entry"/>'s glyphs or vocabularies reads differently in <paramref name="working"/> than when it was written.</summary>
    public static bool IsStale(JournalEntry entry, GrammarDefinition working) =>
        entry.Names.Any(x => Fingerprint(working, x) != entry.Fingerprints.GetValueOrDefault(x, ""));

    /// <summary>Keeps a copy of the journal as it stands, under <paramref name="label"/> - unless it's what the latest copy already holds.</summary>
    public void Snapshot(string label)
    {
        lock (_gate)
        {
            var entries = Order(Load().Entries);

            if (entries.Count == 0 && Versions.Count == 0)
                return;

            if (Versions.FirstOrDefault() is { } latest && DefinitionJson.Serialize(ReadVersion(latest.Id)) == DefinitionJson.Serialize(entries))
                return;

            var at = DateTimeOffset.Now;
            Directory.CreateDirectory(VersionsFolder);
            File.WriteAllText(Path.Combine(VersionsFolder, $"{at:yyyyMMdd-HHmmss-fff}.json"), DefinitionJson.Serialize(new SavedVersion(label, at, entries.ToList())));
        }

        Changed?.Invoke();
    }

    /// <summary>The copies kept by <see cref="Snapshot"/>, newest first.</summary>
    public IReadOnlyList<JournalVersion> Versions
    {
        get
        {
            lock (_gate)
            {
                if (!Directory.Exists(VersionsFolder))
                    return [];

                return Directory.EnumerateFiles(VersionsFolder, "*.json")
                    .Select(x => (Id: Path.GetFileNameWithoutExtension(x), Saved: DefinitionJson.Deserialize<SavedVersion>(File.ReadAllText(x))))
                    .Select(x => new JournalVersion(x.Id, x.Saved.Label, x.Saved.At))
                    .OrderByDescending(x => x.At)
                    .ToList();
            }
        }
    }

    /// <summary>The entries of the copy <paramref name="id"/> (see <see cref="Versions"/>).</summary>
    public IReadOnlyList<JournalEntry> ReadVersion(string id)
    {
        lock (_gate)
        {
            var path = Path.Combine(VersionsFolder, Path.GetFileName(id) + ".json");

            if (!File.Exists(path))
                throw new InvalidOperationException($"There's no journal version '{id}'");

            return Order(Complete(DefinitionJson.Deserialize<SavedVersion>(File.ReadAllText(path)).Entries));
        }
    }

    /// <summary>Puts this journal's entries in place in <paramref name="folder"/> - another workspace's, being created from this one. Its versions stay here.</summary>
    public void CopyTo(string folder)
    {
        lock (_gate)
        {
            if (File.Exists(FilePath))
                File.Copy(FilePath, Path.Combine(folder, _fileName), overwrite: true);
        }
    }

    /// <summary>The entries as text, a section at a time: each with its id, the names it's about, and - against <paramref name="working"/>, when given - whether they've changed since.</summary>
    public static string Render(IReadOnlyList<JournalEntry> entries, GrammarDefinition working = null)
    {
        if (entries.Count == 0)
            return "(empty)";

        var text = new StringBuilder();

        foreach (var section in entries.GroupBy(x => x.Section))
        {
            text.AppendLine($"{SectionTitle(section.Key)}:");

            foreach (var entry in section)
            {
                var about = entry.Names.Count > 0 ? $" [{string.Join(", ", entry.Names)}]" : "";
                var stale = working is not null && IsStale(entry, working) ? " (may be stale: changed since)" : "";
                text.AppendLine($"  #{entry.Id}{about}{stale} {entry.Text.ReplaceLineEndings(" ")}");
            }
        }

        return text.ToString().TrimEnd();
    }

    public static string SectionTitle(JournalSection section) => section switch
    {
        JournalSection.OpenProblem => "Open problems",
        JournalSection.DeadEnd => "Dead ends",
        _ => "Hints",
    };

    /// <summary>A section as written - "open problem", "dead_end", "hints" and the like.</summary>
    public static bool TryParseSection(string text, out JournalSection section)
    {
        var key = new string((text ?? "").Where(char.IsLetter).ToArray()).ToLowerInvariant().TrimEnd('s');

        section = key switch
        {
            "openproblem" or "problem" or "open" => JournalSection.OpenProblem,
            "deadend" or "dead" => JournalSection.DeadEnd,
            "hint" or "note" => JournalSection.Hint,
            _ => (JournalSection)(-1),
        };

        return Enum.IsDefined(section);
    }

    public static int CountWords(string text) =>
        (text ?? "").Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;

    static IReadOnlyList<JournalEntry> Order(IEnumerable<JournalEntry> entries) =>
        entries.OrderBy(x => x.Section).ThenBy(x => x.Id).ToList();

    static string RequireText(string text) =>
        string.IsNullOrWhiteSpace(text) ? throw new InvalidOperationException("A journal entry needs some text") : text.Trim();

    static List<string> CleanNames(IEnumerable<string> names) =>
        (names ?? []).Select(x => x?.Trim()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

    static int IndexOf(Saved saved, int id) =>
        saved.Entries.FindIndex(x => x.Id == id) is int index and >= 0
            ? index
            : throw new InvalidOperationException($"The journal has no entry #{id}");

    static Dictionary<string, string> Fingerprint(GrammarDefinition working, IReadOnlyList<string> names) =>
        names.ToDictionary(x => x, x => Fingerprint(working, x));

    /// <summary>A short hash of how <paramref name="name"/> reads in <paramref name="working"/> - its documentation aside - or empty if it has nothing of that name.</summary>
    static string Fingerprint(GrammarDefinition working, string name)
    {
        static string Hash(string json) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json)))[..12];

        if (working.Glyphs.FirstOrDefault(x => x.Name == name) is { } glyph)
            return Hash(DefinitionJson.Serialize(glyph with { Documentation = null }));

        if (working.Vocabularies.FirstOrDefault(x => x.Name == name) is { } vocabulary)
            return Hash(DefinitionJson.Serialize(vocabulary));

        return working.Markers.Contains(name) ? Hash("marker " + name) : "";
    }

    Saved Load() =>
        File.Exists(FilePath) && DefinitionJson.Deserialize<Saved>(File.ReadAllText(FilePath)) is { } saved
            ? saved with { Entries = Complete(saved.Entries) }
            : new(1, []);

    /// <summary><paramref name="entries"/> as read back, with the empty lists that are left out of the JSON put back.</summary>
    static List<JournalEntry> Complete(List<JournalEntry> entries) =>
        (entries ?? []).Select(x => x with { Names = x.Names ?? [], Fingerprints = x.Fingerprints ?? new Dictionary<string, string>() }).ToList();

    void Save(Saved saved)
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllText(FilePath, DefinitionJson.Serialize(saved));
    }
}
