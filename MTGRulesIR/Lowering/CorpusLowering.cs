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

    public CardRules Lower(ProcessedDocument document)
    {
        var types = (document.Document as MTGGlyphs.Data.Card)?.Types ?? "";
        return new CardRules(document.Document.Name, types, document.Lines.Select(x => LowerLine(document.Document, types, x)).ToList());
    }

    public LineRules LowerLine(IDocument document, string types, ProcessedLine line)
    {
        var ctx = new LoweringContext(document, line);
        List<Ability> abilities = [];

        foreach (var unit in line.Glyphs)
            switch (unit)
            {
                case ClauseBreak:
                    continue;

                case UnmatchedString unmatched:
                    abilities.Add(new OpaqueAbility(unmatched.CaptureValue, "Unmatched"));
                    continue;

                case Glyph glyph:
                    if (_lowerer.Lower(glyph, ctx) is { } node)
                        abilities.Add(_lowerer.AsAbility(node, ctx, glyph));
                    continue;
            }

        var opaque = abilities.SelectMany(OpaqueNodes.In).Select(x => x.Glyph).ToList();
        return new LineRules(line.SourceText.LineIndex, line.SourceText.FormattedText, abilities, opaque);
    }

}

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
