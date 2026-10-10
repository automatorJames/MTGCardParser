using Glyphotype.Distiller.Workbench;

namespace Glyphotype.Distiller.Workspaces;

/// <summary>
/// One definition a merge (see <see cref="WorkspaceMerge"/>) would bring into its target: what the merged workspace
/// did to it since its merge base. Each side's definition is null where it's absent (a marker's is its name).
/// </summary>
/// <param name="Change">What the merged workspace did to it since the merge base.</param>
/// <param name="Base">The definition as it was at the merge base.</param>
/// <param name="Target">The definition as the target has it now.</param>
/// <param name="Incoming">The definition as the merged workspace has it.</param>
/// <param name="IsConflict">Whether the target changed it since the merge base too - differently - so taking it would undo that.</param>
public sealed record MergeItem(DefinitionKind Kind, string Name, ChangeType Change, object Base, object Target, object Incoming, bool IsConflict);

/// <summary>
/// What merging one workspace's grammar into another's would do, for review (see <see cref="WorkspaceManager.PlanMerge"/>):
/// every glyph, vocabulary and marker the merged workspace added, changed or removed since its merge base - where it
/// started, or what it last merged - that the target doesn't already have as it does.
/// </summary>
/// <param name="From">The workspace being merged.</param>
/// <param name="Into">The workspace it's merged into.</param>
/// <param name="Base">What <paramref name="From"/> had when it started, or last merged.</param>
/// <param name="Target"><paramref name="Into"/>'s working definition, which the merge is applied on top of.</param>
/// <param name="Incoming"><paramref name="From"/>'s working definition.</param>
/// <param name="IsBaseRecorded">
/// Whether <paramref name="Base"/> is <paramref name="From"/>'s own merge base - else no record of one was kept, and
/// it's <paramref name="Into"/>'s committed grammar, so whatever differs from that is offered.
/// </param>
public sealed record WorkspaceMerge(string From, string Into, GrammarDefinition Base, GrammarDefinition Target, GrammarDefinition Incoming,
    bool IsBaseRecorded, IReadOnlyList<MergeItem> Items)
{
    /// <summary>The items taken by default: every one that isn't a conflict.</summary>
    public IReadOnlyList<MergeItem> CleanItems => Items.Where(x => !x.IsConflict).ToList();

    public static WorkspaceMerge Plan(string from, string into, GrammarDefinition @base, GrammarDefinition target, GrammarDefinition incoming, bool isBaseRecorded)
    {
        var items = new List<MergeItem>();

        foreach (var change in DefinitionDiff.Compare(@base, incoming))
        {
            var current = Find(target, change.Kind, change.Name);

            // The target has it as the merged workspace does already.
            if (Same(current, change.After))
                continue;

            items.Add(new(change.Kind, change.Name, change.Change, change.Before, current, change.After, IsConflict: !Same(current, change.Before)));
        }

        return new(from, into, @base, target, incoming, isBaseRecorded, items);
    }

    /// <summary><see cref="Target"/> with <paramref name="taken"/> brought in from <see cref="Incoming"/>.</summary>
    /// <exception cref="InvalidOperationException">A definition the result still refers to would be removed.</exception>
    public GrammarDefinition Apply(IEnumerable<MergeItem> taken)
    {
        var result = Target;
        var removed = new List<string>();

        foreach (var item in taken)
        {
            result = With(result, item.Kind, item.Name, item.Incoming);

            if (item.Incoming is null)
                removed.Add(item.Name);
        }

        foreach (var name in removed)
            if (result.GetReferrers(name) is { Count: > 0 } referrers)
                throw new InvalidOperationException(
                    $"{From} removed {name}, but {string.Join(", ", referrers)} still refer{(referrers.Count == 1 ? "s" : "")} to it in what the merge would leave - take {(referrers.Count == 1 ? "that change" : "those changes")} too, or leave {name}");

        return result;
    }

    /// <summary>
    /// The merge base <see cref="From"/> has once <paramref name="taken"/> are merged: <see cref="Incoming"/>, but with
    /// what wasn't taken as it was at the merge base - so the next merge offers it again.
    /// </summary>
    public GrammarDefinition NextBase(IEnumerable<MergeItem> taken)
    {
        var takenKeys = taken.Select(x => (x.Kind, x.Name)).ToHashSet();
        return Items.Where(x => !takenKeys.Contains((x.Kind, x.Name))).Aggregate(Incoming, (result, item) => With(result, item.Kind, item.Name, item.Base));
    }

    static GrammarDefinition With(GrammarDefinition grammar, DefinitionKind kind, string name, object definition) =>
        (kind, definition) switch
        {
            (DefinitionKind.Glyph, GlyphDefinition glyph) => grammar.WithGlyph(glyph),
            (DefinitionKind.Glyph, _) => grammar.WithoutGlyph(name),
            (DefinitionKind.Vocabulary, VocabularyDefinition vocabulary) => grammar.WithVocabulary(vocabulary),
            (DefinitionKind.Vocabulary, _) => grammar.WithoutVocabulary(name),
            (_, null) => grammar with { Markers = grammar.Markers.Where(x => x != name).ToList() },
            _ => grammar with { Markers = grammar.Markers.Union([name]).ToList() },
        };

    static object Find(GrammarDefinition grammar, DefinitionKind kind, string name) =>
        kind switch
        {
            DefinitionKind.Glyph => grammar.Glyphs.FirstOrDefault(x => x.Name == name),
            DefinitionKind.Vocabulary => grammar.Vocabularies.FirstOrDefault(x => x.Name == name),
            _ => grammar.Markers.Contains(name) ? name : null,
        };

    static bool Same(object a, object b) =>
        a is null || b is null ? a is null && b is null : DefinitionJson.Serialize(a) == DefinitionJson.Serialize(b);
}
