namespace MTGCardParser.TokenTesting;

public record PositionalToken
{
    public Card Card { get; init; }
    public int LineIndex { get; init; }
    public int TokenIndex { get; init; }
    public TokenUnit Token { get; init; }
    public string CaptureId { get; init; }
    public List<PositionalToken> Children { get; init; } = [];
    public IReadOnlyList<TokenSegment> Segments { get; init; }

    public PositionalToken(TokenUnit token, Card card, int lineIndex, int tokenIndex, int? childIndex = null)
    {
        Card = card;
        LineIndex = lineIndex;
        TokenIndex = tokenIndex;
        Token = token;
        CaptureId = $"{card.Name}-{card.CardId}-{lineIndex}-{tokenIndex}";

        if (childIndex.HasValue)
            CaptureId += $"-child{childIndex.Value}";

        // The Children list is created here, which is needed for segment digestion.
        foreach (var (child, idx) in token.ChildTokens.OrderBy(c => c.MatchSpan.Position.Absolute).Select((token, index) => (token, index)))
            Children.Add(new(child, card, lineIndex, tokenIndex, idx));

        Segments = DigestSegments();
    }

    /// <summary>
    /// This method contains the logic that was previously in the Razor component.
    /// It breaks the token's text down into a series of PlainTextSegments and ChildTokenSegments.
    /// </summary>
    private IReadOnlyList<TokenSegment> DigestSegments()
    {
        var segments = new List<TokenSegment>();
        var parentSpan = Token.MatchSpan;

        // Get the parent token's property matches as a list.
        // This is done once to provide a stable list with consistent indexing,
        // which is crucial for consistent coloring of the property captures.
        var propMatchesAsList = Token.PropMatches.ToList();

        if (!Children.Any())
        {
            // If there are no children, the token is a single leaf containing the entire text.
            // Pass the property matches to the new leaf.
            segments.Add(new TokenSegmentLeaf(parentSpan.ToStringValue(), parentSpan.Position.Absolute, propMatchesAsList));
            return segments;
        }

        int currentIndexInParentText = 0;

        foreach (var child in Children)
        {
            var childSpan = child.Token.MatchSpan;
            int childRelativeStart = childSpan.Position.Absolute - parentSpan.Position.Absolute;

            // a) Create prefix text segment (which is a leaf)
            if (childRelativeStart > currentIndexInParentText)
            {
                int prefixLength = childRelativeStart - currentIndexInParentText;
                int prefixAbsoluteStart = parentSpan.Position.Absolute + currentIndexInParentText;
                string prefixText = parentSpan.Source!.Substring(prefixAbsoluteStart, prefixLength);

                // Pass the property matches to the new prefix leaf.
                segments.Add(new TokenSegmentLeaf(prefixText, prefixAbsoluteStart, propMatchesAsList));
            }

            // b) Create the child token branch
            segments.Add(new TokenSegmentBranch(child));

            // c) Update position
            currentIndexInParentText = childRelativeStart + childSpan.Length;
        }

        // d) Create suffix text segment (which is a leaf)
        if (currentIndexInParentText < parentSpan.Length)
        {
            int suffixLength = parentSpan.Length - currentIndexInParentText;
            int suffixAbsoluteStart = parentSpan.Position.Absolute + currentIndexInParentText;
            string suffixText = parentSpan.Source!.Substring(suffixAbsoluteStart, suffixLength);

            // Pass the property matches to the new suffix leaf.
            segments.Add(new TokenSegmentLeaf(suffixText, suffixAbsoluteStart, propMatchesAsList));
        }

        return segments;
    }
}