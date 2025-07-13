namespace MTGPlexer.TokenAnalysis.DTOs;

public record class CardLine
{
    Card _card;

    public List<NestedSpanRoot> SpanRoots { get; }
    public int LineIndex { get; }

    public CardLine(Card card, List<Token<Type>> tokens, int lineIndex)
    {
        _card = card;
        LineIndex = lineIndex;
        SpanRoots = GetHydratedTokenUnits(tokens);
    }

    List<NestedSpanRoot> GetHydratedTokenUnits(List<Token<Type>> tokens)
    {
        List<NestedSpanRoot> roots = new();

        foreach (var token in tokens)
        {
            var hydratedTokenUnit = TokenTypeRegistry.HydrateFromToken(token);
            roots.Add(new(hydratedTokenUnit, _card.Name));
        }

        return roots;
    }
}

