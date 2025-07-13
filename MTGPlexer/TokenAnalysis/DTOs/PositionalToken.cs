using System.Diagnostics;

namespace MTGPlexer.TokenAnalysis.DTOs;

public record PositionalToken
{
    public Card Card { get; init; }
    public int LineIndex { get; init; }
    public int Position { get; init; }
    public TokenUnit Token { get; init; }
    public PositionalToken Parent{ get; init; }
    public List<PositionalToken> ChildTokens { get; init; } = [];
    public List<TokenSegment> Segments { get; init; }
    public bool IsComplex { get; init; }
    public bool IgnoreInAnalysis { get; init; }
    public int NestedDepth { get; init; }
    public DeterministicPalette Palette { get; init; }
    public string Path { get; init; }
    public List<ScalarPropVal> PropVals { get; init; }
    public bool IsTransparentInAnalysisDisplay { get; init; }

    public PositionalToken(TokenUnit token, Card card, int lineIndex, int position, PositionalToken parent = null, int? childIndex = null)
    {
        Parent = parent;
        Path = parent != null ? parent.Path : $"{card.Name.Replace(' ', '-')}.line[{lineIndex}].index[{token.MatchSpan.Position.Absolute}]";
        Path += "." + token.Type.Name;
        Card = card;
        LineIndex = lineIndex;
        Position = position;
        Token = token;
        IsComplex = Token is TokenUnitComplex;
        IsTransparentInAnalysisDisplay = Token is TokenUnitOneOf;
        IgnoreInAnalysis = Token.Type.GetCustomAttribute<IgnoreInAnalysisAttribute>() is not null;
        Palette = TokenTypeRegistry.TypeColorPalettes[token.Type];

        if (childIndex.HasValue)
            NestedDepth = childIndex.Value + 1;

        // The list of children is created here, which is needed for segment digestion.
        foreach (var (child, idx) in token.ChildTokens.OrderBy(c => c.MatchSpan.Position.Absolute).Select((token, index) => (token, index)))
            ChildTokens.Add(new(child, card, lineIndex, position, this, idx));

        Segments = GetDigestedSegments();
        PropVals = GetPropVals();
    }

    List<ScalarPropVal> GetPropVals()
    {
        // A token will either only regular prop vals, only distilled prop vals, or neither,
        // and therefore order of processing regular vs. distilled doesn't matter.

        List<ScalarPropVal> list = new();

        var orderedRegexProps = Token
            .Template
            .RegexPropInfos
            .Where(x => x.RegexPropType != RegexPropType.TokenUnit)
            .ToList();

        for (int i = 0; i < orderedRegexProps.Count; i++)
        {
            var captureProp = orderedRegexProps[i];
            var prop = captureProp.Prop;
            var value = prop.GetValue(Token);

            if (value == null) continue;
            if (!prop.PropertyType.IsEnum && prop.PropertyType.IsValueType && value.Equals(Activator.CreateInstance(prop.PropertyType))) continue;
            if (IsComplex && captureProp.RegexPropType == RegexPropType.Placeholder) continue;

            list.Add(new(captureProp, value, i, Path));
        }

        var distilledValues = Token.DistilledValues.ToList();

        for (int i = 0; i < distilledValues.Count; i++)
        {
            var distilledValue = distilledValues[i];
            RegexPropInfo regexPropInfo = new(distilledValue.Key);

            list.Add(new(regexPropInfo, distilledValue.Value, i, Path));
        }

        return list;
    }

    /// <summary>
    /// This method breaks the token's text down into a series of leaves (text) and branches (child tokens).
    /// It leverages the pre-processed property captures from the TokenUnit. Note: this digested heirarchy 
    /// exists in parallel to the already-existing PositionalToken parent -> child heirarchy. This parallel
    /// helper structure is for ease of rendering nested spans of HTML. To put it another way, only the outer
    /// parent PositionalToken is ever directly rendered in HTML, and instead of recursing through its ChildTokens,
    /// The HTML renderer recurses through the parallel structure of TokenSegmentBranches, which represent
    /// pre-digested ordered spans of text.
    /// </summary>
    private List<TokenSegment> GetDigestedSegments()
    {
        var segments = new List<TokenSegment>();
        var parentSpan = Token.MatchSpan;

        if (!ChildTokens.Any())
        {
            // If there are no children, the token is a single leaf containing the entire text.
            // Pass the clean, pre-processed list to the new leaf.
            segments.Add(new TokenSegmentLeaf(parentSpan.ToStringValue(), parentSpan.Position.Absolute, Token, Path));
            return segments;
        }

        int currentIndexInParentText = 0;

        foreach (var child in ChildTokens)
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
                segments.Add(new TokenSegmentLeaf(prefixText, prefixAbsoluteStart, Token, Path));
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
            segments.Add(new TokenSegmentLeaf(suffixText, suffixAbsoluteStart, Token, Path));
        }

        return segments;
    }

    public override string ToString() => Token.ToString();
}