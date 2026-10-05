namespace Glyphotype.Definitions;

/// <summary>
/// A set of <see cref="GlyphDefinition"/>s and the vocabularies and markers they refer to - the portable,
/// serializable source of a grammar, which <see cref="GlyphGrammar.FromDefinition"/> compiles into a runnable
/// <see cref="GlyphGrammar"/>. A reference to a name defined nowhere here must be resolvable when it's compiled
/// (see <see cref="GrammarEmitter.Emit"/>).
/// </summary>
public sealed record GrammarDefinition
{
    public IReadOnlyList<GlyphDefinition> Glyphs { get; init; } = [];

    public IReadOnlyList<VocabularyDefinition> Vocabularies { get; init; } = [];

    /// <summary>
    /// Marker interfaces: member-less tags a glyph can carry (<see cref="GlyphDefinition.Markers"/>) so a
    /// <see cref="DynamicGlyph"/> property's <see cref="PropertyDefinition.TypeFilter"/> can select it.
    /// </summary>
    public IReadOnlyList<string> Markers { get; init; } = [];

    /// <summary>
    /// The names of Glyphotype's own enums and glyphs (e.g. <see cref="Conjunction"/>, <see cref="It"/>): a grammar
    /// refers to them without defining them, and they always resolve when it's compiled (see <see cref="GrammarEmitter.Emit"/>).
    /// </summary>
    public static IReadOnlySet<string> BuiltInNames { get; } =
        typeof(Glyph).Assembly.GetExportedTypes().Where(GrammarEmitter.IsBuiltIn).Select(x => x.Name).ToHashSet();

    /// <summary>Reads the definitions of <paramref name="glyphTypes"/>, plus the vocabularies and markers they refer to. Generic primitives among them are skipped: they're referred to, not defined.</summary>
    public static GrammarDefinition FromTypes(IEnumerable<Type> glyphTypes) =>
        DefinitionReader.ReadGrammar(glyphTypes);

    public string ToJson() => DefinitionJson.Serialize(this);

    public static GrammarDefinition FromJson(string json) => DefinitionJson.Deserialize<GrammarDefinition>(json);
}
