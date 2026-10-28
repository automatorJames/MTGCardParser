using System.Collections;
using System.Collections.Concurrent;

namespace MTGRulesIR.Lowering;

/// <summary>Lowers whole documents: each line's top-level glyphs to abilities, classified by the card's types where the glyph alone doesn't say what frame it is.</summary>
public sealed class CorpusLowering
{
    readonly Lowerer _lowerer;

    public CorpusLowering(Lowerer lowerer = null)
    {
        _lowerer = lowerer ?? new Lowerer();
    }

    public Lowerer Lowerer => _lowerer;

    public CardRules Lower(ProcessedDocument document) =>
        LowerWithOrigins(document).Rules;

    /// <summary>
    /// <paramref name="document"/> lowered, with where in each line's text each node came from, for the nodes whose
    /// source is known: the ones a glyph lowered to, and the line's roots.
    /// </summary>
    public (CardRules Rules, IReadOnlyList<LineOrigins> Origins) LowerWithOrigins(ProcessedDocument document)
    {
        var types = (document.Document as MTGGlyphs.Data.Card)?.Types ?? "";
        List<LineRules> lines = [];
        List<LineOrigins> origins = [];

        foreach (var line in document.Lines)
        {
            var (rules, lineOrigins) = LowerLineWithOrigins(document.Document, types, line);
            lines.Add(rules);
            origins.Add(lineOrigins);
        }

        return (new CardRules(document.Document.Name, types, lines), origins);
    }

    public LineRules LowerLine(IDocument document, string types, ProcessedLine line) =>
        LowerLineWithOrigins(document, types, line).Rules;

    (LineRules Rules, LineOrigins Origins) LowerLineWithOrigins(IDocument document, string types, ProcessedLine line)
    {
        var ctx = new LoweringContext(document, line);
        List<Ability> abilities = [];

        foreach (var unit in line.Glyphs)
            switch (unit)
            {
                case ClauseBreak:
                    continue;

                case UnmatchedString unmatched:
                    var unlowered = new OpaqueAbility(unmatched.CaptureValue, "Unmatched");
                    ctx.Record(unlowered, unmatched);
                    abilities.Add(unlowered);
                    continue;

                case Glyph glyph:
                    if (_lowerer.Lower(glyph, ctx) is { } node)
                    {
                        var ability = _lowerer.AsAbility(node, ctx, glyph);
                        ctx.Record(ability, glyph);
                        abilities.Add(ability);
                    }
                    continue;
            }

        var opaque = abilities.SelectMany(OpaqueNodes.In).Select(x => x.Glyph).ToList();
        return (new LineRules(line.SourceText.LineIndex, line.SourceText.FormattedText, abilities, opaque), new LineOrigins(ctx.Origins, ctx.UnboundReferences));
    }

}

/// <summary>Where one line's nodes came from in its text (see <see cref="LoweringContext.Origins"/>), and how many references stayed unbound.</summary>
public sealed record LineOrigins(IReadOnlyDictionary<IrNode, Glyphotype.Lowering.SourceSpan> Spans, int UnboundReferences);

/// <summary>Finds the opaque nodes in an IR tree, by walking every node-typed property and list.</summary>
public static class OpaqueNodes
{
    static readonly ConcurrentDictionary<Type, PropertyInfo[]> _properties = new();

    public static IEnumerable<IOpaque> In(IrNode node)
    {
        if (node is null)
            yield break;

        if (node is IOpaque opaque)
            yield return opaque;

        foreach (var property in _properties.GetOrAdd(node.GetType(), x => x.GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray()))
        {
            var value = property.GetValue(node);

            if (value is IrNode child)
                foreach (var found in In(child))
                    yield return found;
            else if (value is IEnumerable list and not string)
                foreach (var item in list)
                    if (item is IrNode element)
                        foreach (var found in In(element))
                            yield return found;
        }
    }
}
