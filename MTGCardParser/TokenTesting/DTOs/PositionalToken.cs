// PositionalToken.cs
namespace MTGCardParser.TokenTesting.DTOs;

public record PositionalToken
{
    public Card Card { get; init; }
    public int LineIndex { get; init; }
    public int TokenIndex { get; init; }
    public TokenUnit Token { get; init; }
    public string CaptureId { get; init; }
    public List<PositionalToken> Children { get; init; } = [];
    public List<TokenSegment> Segments { get; init; }
    public bool IsComplex { get; init; }

    public PositionalToken(TokenUnit token, Card card, int lineIndex, int tokenIndex, int? childIndex = null)
    {
        Card = card;
        LineIndex = lineIndex;
        TokenIndex = tokenIndex;
        Token = token;
        CaptureId = $"{card.Name.Replace(' ', '-')}-{card.CardId}-{lineIndex}-{tokenIndex}";
        IsComplex = Token is TokenUnitComplex;

        if (childIndex.HasValue)
            CaptureId += $"-child{childIndex.Value}";

        // The Children list is created here, which is needed for segment digestion.
        foreach (var (child, idx) in token.ChildTokens.OrderBy(c => c.MatchSpan.Position.Absolute).Select((token, index) => (token, index)))
            Children.Add(new(child, card, lineIndex, tokenIndex, idx));

        Segments = DigestSegments();
    }

    /// <summary>
    /// This method breaks the token's text down into a series of leaves (text) and branches (child tokens).
    /// It leverages the pre-processed property captures from the TokenUnit.
    /// </summary>
    private List<TokenSegment> DigestSegments()
    {
        var segments = new List<TokenSegment>();
        var parentSpan = Token.MatchSpan;

        if (!Children.Any())
        {
            // If there are no children, the token is a single leaf containing the entire text.
            // Pass the clean, pre-processed list to the new leaf.
            segments.Add(new TokenSegmentLeaf(parentSpan.ToStringValue(), parentSpan.Position.Absolute, Token));
            return segments;
        }

        int currentIndexInParentText = 0;

        foreach (var child in Children)
        {
            var childSpan = child.Token.MatchSpan;
            int childRelativeStart = childSpan.Position.Absolute - parentSpan.Position.Absolute;

            // a) Create prefix text segment (a leaf)
            if (childRelativeStart > currentIndexInParentText)
            {
                int prefixLength = childRelativeStart - currentIndexInParentText;
                int prefixAbsoluteStart = parentSpan.Position.Absolute + currentIndexInParentText;
                string prefixText = parentSpan.Source!.Substring(prefixAbsoluteStart, prefixLength);

                // Pass the clean, pre-processed list to the new prefix leaf.
                segments.Add(new TokenSegmentLeaf(prefixText, prefixAbsoluteStart, Token));
            }

            // b) Create the child token branch
            segments.Add(new TokenSegmentBranch(child));

            // c) Update position
            currentIndexInParentText = childRelativeStart + childSpan.Length;
        }

        // d) Create suffix text segment (a leaf)
        if (currentIndexInParentText < parentSpan.Length)
        {
            int suffixLength = parentSpan.Length - currentIndexInParentText;
            int suffixAbsoluteStart = parentSpan.Position.Absolute + currentIndexInParentText;
            string suffixText = parentSpan.Source!.Substring(suffixAbsoluteStart, suffixLength);

            // Pass the clean, pre-processed list to the new suffix leaf.
            segments.Add(new TokenSegmentLeaf(suffixText, suffixAbsoluteStart, Token));
        }

        return segments;
    }
}