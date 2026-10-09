namespace Glyphotype.Distiller.Workbench;

/// <summary>Non-destructive edits to a <see cref="GrammarDefinition"/>: each returns a new definition.</summary>
public static class GrammarDefinitionEdits
{
    /// <summary><paramref name="grammar"/> with <paramref name="glyph"/> in place of the glyph named <paramref name="replacing"/> (by default, its own name), or added if there's none.</summary>
    public static GrammarDefinition WithGlyph(this GrammarDefinition grammar, GlyphDefinition glyph, string replacing = null) =>
        grammar with { Glyphs = Replace(grammar.Glyphs, x => x.Name, replacing ?? glyph.Name, glyph) };

    public static GrammarDefinition WithoutGlyph(this GrammarDefinition grammar, string name) =>
        grammar with { Glyphs = grammar.Glyphs.Where(x => x.Name != name).ToList() };

    /// <summary><paramref name="grammar"/> with <paramref name="vocabulary"/> in place of the vocabulary named <paramref name="replacing"/> (by default, its own name), or added if there's none.</summary>
    public static GrammarDefinition WithVocabulary(this GrammarDefinition grammar, VocabularyDefinition vocabulary, string replacing = null) =>
        grammar with { Vocabularies = Replace(grammar.Vocabularies, x => x.Name, replacing ?? vocabulary.Name, vocabulary) };

    public static GrammarDefinition WithoutVocabulary(this GrammarDefinition grammar, string name) =>
        grammar with { Vocabularies = grammar.Vocabularies.Where(x => x.Name != name).ToList() };

    /// <summary><paramref name="grammar"/> with the glyph, vocabulary or marker named <paramref name="name"/> named <paramref name="newName"/> instead, and every reference to it too.</summary>
    public static GrammarDefinition WithRenamed(this GrammarDefinition grammar, string name, string newName)
    {
        string Rename(string x) => x == name ? newName : x;

        TypeReference RenameReference(TypeReference reference) =>
            reference is null ? null
            : reference with
            {
                Name = reference.Kind is TypeReferenceKind.Glyph or TypeReferenceKind.Vocabulary ? Rename(reference.Name) : reference.Name,
                Arguments = reference.Arguments.Select(RenameReference).ToList(),
            };

        return grammar with
        {
            Glyphs = grammar.Glyphs.Select(glyph => glyph with
            {
                Name = Rename(glyph.Name),
                AliasOf = RenameReference(glyph.AliasOf),
                ReferenceKind = RenameReference(glyph.ReferenceKind),
                Markers = glyph.Markers.Select(Rename).ToList(),
                Properties = glyph.Properties.Select(x => x with { Type = RenameReference(x.Type), TypeFilter = x.TypeFilter is null ? null : Rename(x.TypeFilter) }).ToList(),
            }).ToList(),
            Vocabularies = grammar.Vocabularies.Select(x => x with { Name = Rename(x.Name) }).ToList(),
            Markers = grammar.Markers.Select(Rename).ToList(),
        };
    }

    /// <summary>The glyphs in <paramref name="grammar"/> that refer to the glyph, vocabulary or marker named <paramref name="name"/>.</summary>
    public static IReadOnlyList<string> GetReferrers(this GrammarDefinition grammar, string name) =>
        grammar.Glyphs
            .Where(x => x.Name != name && GetReferencedNames(x).Contains(name))
            .Select(x => x.Name)
            .ToList();

    /// <summary>
    /// <paramref name="grammar"/> without the vocabularies and markers no glyph refers to - what reading the same
    /// grammar back from compiled types would give (see <see cref="GrammarDefinition.FromTypes"/>), so the unused
    /// ones neither cost grammar bits nor differ from a committed grammar that still declares them.
    /// </summary>
    public static GrammarDefinition WithoutUnreferencedTerminals(this GrammarDefinition grammar)
    {
        var referenced = grammar.Glyphs.SelectMany(GetReferencedNames).ToHashSet();

        return grammar with
        {
            Vocabularies = grammar.Vocabularies.Where(x => referenced.Contains(x.Name)).ToList(),
            Markers = grammar.Markers.Where(referenced.Contains).ToList(),
        };
    }

    /// <summary>Every glyph, vocabulary and marker name <paramref name="glyph"/> refers to.</summary>
    public static IEnumerable<string> GetReferencedNames(GlyphDefinition glyph) =>
        glyph.Properties.Select(x => x.Type)
            .Append(glyph.AliasOf)
            .Append(glyph.ReferenceKind)
            .Where(x => x is not null)
            .SelectMany(x => x.SelfAndDescendants())
            .Where(x => x.Kind is TypeReferenceKind.Glyph or TypeReferenceKind.Vocabulary)
            .Select(x => x.Name)
            .Concat(glyph.Markers)
            .Concat(glyph.Properties.Select(x => x.TypeFilter).Where(x => x is not null))
            .Distinct();

    static List<T> Replace<T>(IReadOnlyList<T> items, Func<T, string> nameOf, string name, T replacement)
    {
        var result = items.ToList();
        var index = result.FindIndex(x => nameOf(x) == name);

        if (index >= 0)
            result[index] = replacement;
        else
            result.Add(replacement);

        return result;
    }
}
