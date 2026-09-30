namespace Glyphotype.Distiller.Workbench;

public enum DefinitionKind
{
    Glyph,
    Vocabulary,
    Marker,
}

public enum ChangeType
{
    Added,
    Modified,
    Removed,
}

/// <summary>One definition that differs between two grammars. <see cref="Before"/>/<see cref="After"/> are the definitions themselves (a marker's is its name), null on the side it's absent from.</summary>
public sealed record DefinitionChange(DefinitionKind Kind, string Name, ChangeType Change, object Before, object After);

/// <summary>What differs, definition by definition, between two grammars.</summary>
public static class DefinitionDiff
{
    /// <summary>Every glyph, vocabulary and marker added to, modified in or removed from <paramref name="from"/> in <paramref name="to"/>, matched by name and compared structurally.</summary>
    public static IReadOnlyList<DefinitionChange> Compare(GrammarDefinition from, GrammarDefinition to) =>
        [
            .. Compare(DefinitionKind.Glyph, from.Glyphs, to.Glyphs, x => x.Name),
            .. Compare(DefinitionKind.Vocabulary, from.Vocabularies, to.Vocabularies, x => x.Name),
            .. Compare(DefinitionKind.Marker, from.Markers, to.Markers, x => x),
        ];

    static IEnumerable<DefinitionChange> Compare<T>(DefinitionKind kind, IReadOnlyList<T> from, IReadOnlyList<T> to, Func<T, string> nameOf)
    {
        var before = from.ToDictionary(nameOf);
        var after = to.ToDictionary(nameOf);

        foreach (var (name, definition) in after)
        {
            if (!before.TryGetValue(name, out var original))
                yield return new(kind, name, ChangeType.Added, null, definition);
            else if (DefinitionJson.Serialize(original) != DefinitionJson.Serialize(definition))
                yield return new(kind, name, ChangeType.Modified, original, definition);
        }

        foreach (var (name, definition) in before)
            if (!after.ContainsKey(name))
                yield return new(kind, name, ChangeType.Removed, definition, null);
    }
}
