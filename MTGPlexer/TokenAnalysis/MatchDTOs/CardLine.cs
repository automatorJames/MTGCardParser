namespace MTGPlexer.TokenAnalysis.DTOs;

public record class CardLine
{
    public Card Card { get; }
    public string SourceText { get; }
    public List<SpanRoot> SpanRoots { get; }
    public List<SpanContext> UnmatchedSpans { get; } = [];
    public List<Token<Type>> SourceTokens { get; } = [];
    public Dictionary<Type, int> TokenCounts { get; } = [];
    public int LineIndex { get; }

    public CardLine(Card card, string sourceText, List<Token<Type>> tokens, int lineIndex)
    {
        Card = card;
        SourceText = sourceText;
        LineIndex = lineIndex;
        SourceTokens = tokens;
        SpanRoots = GetHydratedTokenUnits(tokens);
        CountTokenTypes(tokens);
    }

    List<SpanRoot> GetHydratedTokenUnits(List<Token<Type>> tokens)
    {
        List<SpanRoot> roots = new();

        // A buffer to hold a preceding text attached to the next root (if any)
        string textToPrecedeNext = null;

        // Track tokens with "EnclosesTokens" (like double quotes) to ensure correct attachment
        Dictionary<Type, int> enclosingTokenCountPerType = new();

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (token.Kind == typeof(DefaultUnmatchedString))
            {
                Token<Type>? precedingToken = i == 0 ? null : tokens[i - 1];
                Token<Type>? followingToken = i == tokens.Count - 1 ? null : tokens[i + 1];
                UnmatchedSpans.Add(new SpanContext(Card.Name, precedingToken, token, followingToken));
            }

            var hydratedTokenUnit = TokenTypeRegistry.HydrateFromToken(token);

            var root = new SpanRoot(hydratedTokenUnit, Card.Name, textToPrecedeNext);
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

