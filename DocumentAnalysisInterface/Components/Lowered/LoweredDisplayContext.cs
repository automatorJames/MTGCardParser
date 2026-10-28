using DocumentAnalysisInterface.Components.CorpusCaptures;
using DocumentAnalysisInterface.PresentationRules;
using Glyphotype.Lowering;

namespace DocumentAnalysisInterface.Components.Lowered;

/// <summary>
/// What every node on the Lowered page draws with: a color per family of the engine's (equidistant hues, in the
/// engine's order, so a family keeps its color), fixed colors for the statuses that aren't a family's, the glyph
/// previews an opaque node's glyph opens, and the grammar an opaque node's glyph name is looked up in.
/// </summary>
public sealed class LoweredDisplayContext
{
    public IReadOnlyDictionary<string, HexPalette> FamilyPalettes { get; }
    public HexPalette OpaquePalette { get; } = DeterministicPalette.GetStaticPalette(LoweredLayout.OpaqueColor);
    public HexPalette TermPalette { get; } = DeterministicPalette.GetStaticPalette(LoweredLayout.TermColor);
    public HexPalette ReferencePalette { get; } = DeterministicPalette.GetStaticPalette(LoweredLayout.ReferenceColor);
    public GlyphPreviews Previews { get; }
    readonly GlyphGrammar _grammar;

    public LoweredDisplayContext(ILoweringEngine engine, GlyphGrammar grammar, GlyphPreviews previews)
    {
        FamilyPalettes = DeterministicPalette.GetPositionalPaletteSet(engine.Families);
        _grammar = grammar;
        Previews = previews;
    }

    /// <summary>The palette <paramref name="node"/> is drawn in: its status's, or for a lowered node its family's.</summary>
    public HexPalette PaletteOf(LoweredNode node) =>
        node.Status switch
        {
            LoweredNodeStatus.Opaque => OpaquePalette,
            LoweredNodeStatus.Term => TermPalette,
            LoweredNodeStatus.Reference => ReferencePalette,
            _ => FamilyPalettes.TryGetValue(node.Family ?? "", out var palette) ? palette : FamilyPalettes.Values.Last(),
        };

    /// <summary>The glyph type named by an opaque node's note, when the grammar has one of that name - what its preview shows.</summary>
    public Type GlyphTypeNamed(string name) =>
        name is not null && _grammar.TryGetType(name, out var type) ? type : GlyphPreviews.BuiltInType(name);

    /// <summary>The key a node and the text it came from share, so hovering one lights the other.</summary>
    public static string SpanKey(SourceSpan? span) =>
        span is { } s ? $"s{s.Index}_{s.Length}" : null;
}
