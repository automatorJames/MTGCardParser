namespace Glyphotype.Distiller.Workbench;

/// <summary>
/// Edits applied together, as one step: definitions to add or replace (each by its own name), and names to remove.
/// Checked as a whole - a change set that leaves any glyph referring to something undefined is refused - so a
/// glyph and the vocabulary it introduces, or a rename and the glyphs that refer to the new name, go in together.
/// </summary>
public sealed record ChangeSet
{
    public SourceDeclarations Declarations { get; init; } = new([], [], []);

    /// <summary>Glyphs, vocabularies or markers to remove, by name.</summary>
    public IReadOnlyList<string> Removals { get; init; } = [];

    public bool IsEmpty => Declarations.IsEmpty && Removals.Count == 0;

    /// <summary>The change set declaring whatever <paramref name="source"/> declares (see <see cref="GlyphSourceReader"/>, which resolves its type names against <paramref name="context"/>), and removing <paramref name="removals"/>.</summary>
    public static ChangeSet FromSource(string source, IEnumerable<string> removals, GrammarDefinition context) =>
        new()
        {
            Declarations = string.IsNullOrWhiteSpace(source) ? new([], [], []) : GlyphSourceReader.Read(source, context),
            Removals = removals?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct().ToList() ?? [],
        };

    /// <summary>
    /// <paramref name="grammar"/> with the removals made, then the declarations added or replaced.
    /// </summary>
    /// <exception cref="InvalidOperationException">A removal names nothing, a declaration's name is taken by a different kind of definition, or the result refers to something undefined.</exception>
    public GrammarDefinition ApplyTo(GrammarDefinition grammar)
    {
        List<string> problems = [];

        foreach (var name in Removals)
        {
            if (grammar.Glyphs.Any(x => x.Name == name))
                grammar = grammar.WithoutGlyph(name);
            else if (grammar.Vocabularies.Any(x => x.Name == name))
                grammar = grammar.WithoutVocabulary(name);
            else if (grammar.Markers.Contains(name))
                grammar = grammar with { Markers = grammar.Markers.Where(x => x != name).ToList() };
            else
                problems.Add($"there's no {name} to remove");
        }

        foreach (var marker in Declarations.Markers.Where(x => !grammar.Markers.Contains(x)))
            grammar = grammar with { Markers = [.. grammar.Markers, marker] };

        foreach (var vocabulary in Declarations.Vocabularies)
            grammar = grammar.WithVocabulary(vocabulary);

        foreach (var glyph in Declarations.Glyphs)
            grammar = grammar.WithGlyph(glyph);

        var kinds = grammar.Glyphs.Select(x => (x.Name, Kind: "glyph"))
            .Concat(grammar.Vocabularies.Select(x => (x.Name, Kind: "vocabulary")))
            .Concat(grammar.Markers.Select(x => (Name: x, Kind: "marker")))
            .ToLookup(x => x.Name, x => x.Kind);

        foreach (var name in kinds.Where(x => x.Count() > 1).Select(x => x.Key))
            problems.Add($"{name} would be both a {string.Join(" and a ", kinds[name])} - names are shared by every kind of definition");

        foreach (var glyph in grammar.Glyphs)
        {
            var missing = GrammarDefinitionEdits.GetReferencedNames(glyph).Where(x => !kinds.Contains(x) && !GrammarDefinition.BuiltInNames.Contains(x)).ToList();

            if (missing.Count > 0)
                problems.Add($"{glyph.Name} refers to {string.Join(", ", missing)}, which {(missing.Count == 1 ? "isn't" : "aren't")} defined{(Removals.Intersect(missing).Any() ? " (removed in this change set)" : "")}");
        }

        if (problems.Count > 0)
            throw new InvalidOperationException(string.Join(Environment.NewLine, problems));

        return grammar;
    }

    /// <summary>A one-line summary of what this changes, e.g. "set AnimalNaps, Pet; remove AnimalRests".</summary>
    public string Describe()
    {
        List<string> parts = [];

        if (!Declarations.IsEmpty)
            parts.Add("set " + string.Join(", ", Declarations.Names));

        if (Removals.Count > 0)
            parts.Add("remove " + string.Join(", ", Removals));

        return parts.Count > 0 ? string.Join("; ", parts) : "no changes";
    }
}
