namespace MTGPlexer.TokenAnalysis.DTOs;

public record class CardLine
{
    Card _card;

    public List<SpanRoot> SpanRoots { get; }
    public HashSet<string> UnmatchedSpans { get; } = [];
    public Dictionary<Type, int> TokenCounts { get; } = [];
    public int LineIndex { get; }

    public CardLine(Card card, List<Token<Type>> tokens, int lineIndex)
    {
        _card = card;
        LineIndex = lineIndex;
        SpanRoots = GetHydratedTokenUnits(tokens);
        CountTokenTypes(tokens);
    }

    List<SpanRoot> GetHydratedTokenUnits(List<Token<Type>> tokens)
    {
        List<SpanRoot> roots = new();

        // A buffer to hold a preceding text attached to the next root (if any)
        string textToPrecedeNext = null;

        // Track tokensd with "EnclosesTokens" (like double quotes) to ensure correct attachment
        Dictionary<Type, int> enclosingTokenCountPerType = new();

        foreach (var token in tokens)
        {
            if (token.Kind == typeof(DefaultUnmatchedString))
                UnmatchedSpans.Add(token.Span.ToStringValue());

            var hydratedTokenUnit = TokenTypeRegistry.HydrateFromToken(token);

            var root = new SpanRoot(hydratedTokenUnit, _card.Name, textToPrecedeNext);
            textToPrecedeNext = null;

            if (root.Placement == TokenPlacement.FollowsPrevious)
                AttachRootTextToPreviousOrNext(root, isNext: false);
            else if (root.Placement == TokenPlacement.PrecedesNext)
                AttachRootTextToPreviousOrNext(root, isNext: true);
            else if (root.Placement == TokenPlacement.AlternatesFollowingAndPreceding)
            {
                if (!enclosingTokenCountPerType.ContainsKey(hydratedTokenUnit.Type))
                    enclosingTokenCountPerType[hydratedTokenUnit.Type] = 0;

                enclosingTokenCountPerType[hydratedTokenUnit.Type]++;
                var isNext = enclosingTokenCountPerType[hydratedTokenUnit.Type] % 2 != 0;
                AttachRootTextToPreviousOrNext(root, isNext: isNext);
            }
            else
                roots.Add(root);
        }

        return roots;

        // Local helper to attach or append following text to the previous root or the next one
        void AttachRootTextToPreviousOrNext(SpanBranch spanWithTextToAttach, bool isNext)
        {
            if (!isNext && !roots.Any())
                return;

            if (isNext)
                textToPrecedeNext = (textToPrecedeNext ?? "") + spanWithTextToAttach.Text;
            else
            {
                var appendedText = (roots[^1].AttachedFollowingText ?? "") + spanWithTextToAttach.Text;
                roots[^1] = roots[^1] with { AttachedFollowingText = appendedText };
            }
        }
    }

    void CountTokenTypes(List<Token<Type>> tokens)
    {
        foreach (var token in tokens)
        {
            if (!TokenCounts.ContainsKey(token.Kind))
                TokenCounts[token.Kind] = 0;

            TokenCounts[token.Kind]++;
        }
    }

    public override string ToString() =>
        string.Join(' ', SpanRoots.Select(x => x.ToString()));
}

