using System.Runtime.CompilerServices;

namespace Glyphotype.StaticRegistry;

/// <summary>
/// Process-wide memo of what's derived from a single Glyph type's own definition - its
/// <see cref="GlyphTypeConfiguration"/> (Nibs, Joiner) and its <see cref="RegexGraph"/>. Both are pure functions of
/// the type, identical in every <see cref="GlyphGrammar"/> it appears in, so they're built once and shared. That's
/// also what lets the deep internals reach them without a grammar in hand - e.g. <see cref="Glyph.Prop"/> building a
/// <see cref="PropertyNib"/> whose <see cref="Navigation"/> needs the nested type's configuration.
/// <para>
/// Everything that depends on a *set* of types - discovery, validation, tokenization order, settings - belongs to
/// <see cref="GlyphGrammar"/> instead.
/// </para>
/// <para>
/// Building a graph for a type with a circular property reference recurses without end; a <see cref="GlyphGrammar"/>
/// rules that out for its types before building any (see <see cref="Glyph.CheckForReferenceLoops(Type)"/>).
/// </para>
/// <para>
/// Held weakly, by type: what's cached for a type emitted at runtime (see <see cref="GrammarEmitter"/>) goes once the
/// type itself is no longer used, rather than keeping it, and its assembly, alive for good.
/// </para>
/// </summary>
public static class GlyphTypeCache
{
    // Built under one reentrant lock (building a type's configuration can build its nested types' configurations)
    // and read lock-free: graphs may be first requested from parallel tokenization.
    static readonly object _buildGate = new();
    static readonly ConditionalWeakTable<Type, GlyphTypeConfiguration> _configurations = new()
    {
        // Glyph itself is abstract, so it can't be instantiated to read its Nibs.
        { typeof(Glyph), new GlyphTypeConfiguration(typeof(Glyph), [], Joiner.Space, []) },
    };
    static readonly ConditionalWeakTable<Type, RegexGraph> _graphs = new();

    public static GlyphTypeConfiguration GetConfiguration(Type glyphType)
    {
        // DynamicGlyphs have no nibs b/c it contains an Item object that will be resolved via the Tokenizer at runtime
        if (glyphType.IsAssignableTo(typeof(DynamicGlyph)))
            return new(glyphType, [], Joiner.None, []);

        if (_configurations.TryGetValue(glyphType, out var configuration))
            return configuration;

        lock (_buildGate)
            return _configurations.TryGetValue(glyphType, out configuration)
                ? configuration
                : Add(_configurations, glyphType, BuildConfiguration(glyphType));
    }

    public static RegexGraph GetRegexGraph(Type type)
    {
        if (_graphs.TryGetValue(type, out var graph))
            return graph;

        lock (_buildGate)
            return _graphs.TryGetValue(type, out graph)
                ? graph
                : Add(_graphs, type, RegexGraph.Create(type));
    }

    static T Add<T>(ConditionalWeakTable<Type, T> table, Type type, T value) where T : class
    {
        table.AddOrUpdate(type, value);
        return value;
    }

    static GlyphTypeConfiguration BuildConfiguration(Type glyphType)
    {
        var instance = (Glyph)Activator.CreateInstance(glyphType);
        var nibs = instance.Nibs.ToArray();

        if (nibs.Length == 0)
        {
            var propertyNibs = PropertyNib.GetPropertyNibs(glyphType);

            if (propertyNibs.Length > 0)
                nibs = propertyNibs;
            else if (glyphType.GetCustomAttribute<RegexPatternAttribute>() is RegexPatternAttribute attr)
                nibs = attr.Patterns.Select(x => new PatternNib(x)).ToArray();
            else
                nibs = [new Nib(glyphType.Name.ToFriendlyCase(TitleDisplayOption.Lower))];
        }

        return new(glyphType, PluralNib.Flatten(nibs), instance.Joiner, nibs);
    }
}
