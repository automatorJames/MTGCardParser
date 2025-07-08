namespace MTGCardParser.TokenTesting;

public record PositionalToken
{
    public Card Card { get; init; }
    public int LineIndex { get; init; }
    public int TokenIndex { get; init; }
    public TokenUnit Token { get; init; }
    public string CaptureId { get; init; }
    public List<PositionalToken> Children { get; init; } = [];

    // New Property: Holds the digested segments of this token.
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

        // The core logic is moved here.
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

        if (!Children.Any())
        {
            // If there are no children, the token is a leaf
            segments.Add(new TokenSegmentLeaf(parentSpan.ToStringValue(), parentSpan.Position.Absolute));
            return segments;
        }

        int currentIndexInParentText = 0;

        foreach (var child in Children)
        {
            var childSpan = child.Token.MatchSpan;
            int childRelativeStart = childSpan.Position.Absolute - parentSpan.Position.Absolute;

            // a) Create prefix text segment
            if (childRelativeStart > currentIndexInParentText)
            {
                int prefixLength = childRelativeStart - currentIndexInParentText;
                int prefixAbsoluteStart = parentSpan.Position.Absolute + currentIndexInParentText;
                string prefixText = parentSpan.Source!.Substring(prefixAbsoluteStart, prefixLength);
                segments.Add(new TokenSegmentLeaf(prefixText, prefixAbsoluteStart));
            }

            // b) Create the child token banch
            segments.Add(new TokenSegmentBranch(child));

            // c) Update position
            currentIndexInParentText = childRelativeStart + childSpan.Length;
        }

        // d) Create suffix text segment
        if (currentIndexInParentText < parentSpan.Length)
        {
            int suffixLength = parentSpan.Length - currentIndexInParentText;
            int suffixAbsoluteStart = parentSpan.Position.Absolute + currentIndexInParentText;
            string suffixText = parentSpan.Source!.Substring(suffixAbsoluteStart, suffixLength);
            segments.Add(new TokenSegmentLeaf(suffixText, suffixAbsoluteStart));
        }

        return segments;
    }
}