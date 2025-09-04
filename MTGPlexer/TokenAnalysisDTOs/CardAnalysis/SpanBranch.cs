using System.Collections;

namespace MTGPlexer.TokenAnalysisDTOs.CardAnalysis;

public record SpanBranch : NestedSpan
{
    public string CardName { get; set; }
    public string DisplayName { get; set; }
    public List<NestedSpan> Children { get; }
    public List<SpanBranch> Branches { get; }
    public List<SpanLeaf> Leaves { get; }
    public List<SpanLeaf> LeavesOrDistilled { get; private set; } = [];
    public TextSpan TokenSpan { get; }
    public Type TokenType { get; }
    public bool CollapseInAnalysis { get; }
    public string OriginalLineText { get; }
    public int? ManyIndex { get; }
    public string Text => TokenSpan.ToStringValue().Trim();

    public SpanBranch(TokenUnit token, string cardName, string parentPath, int parentDepth, string originalLineText, int? manyIndex = null)
        : base(
            Path: parentPath.Dot(token.MatchSpan.ToIndexString()).Dot(token.Type.Name),
            NestedDepth: parentDepth + 1,
            Palette: TokenTypeRegistry.Palettes[token.Type],
            IgnoreInAnalysis: token.Type.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null)
    {
        OriginalLineText = originalLineText;
        CardName = cardName;
        DisplayName = token.Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        Children = DigestChildren(token);
        Branches = Children.OfType<SpanBranch>().ToList();
        Leaves = Children.OfType<SpanLeaf>().ToList();
        SetLeavesOrDistilled(token);
        TokenSpan = token.MatchSpan;
        TokenType = token.Type;
        ManyIndex = manyIndex;
        CollapseInAnalysis = token is TokenUnitOneOf;
    }

    private List<NestedSpan> DigestChildren(TokenUnit token)
    {
        var parentSpan = token.MatchSpan;
        var parentSpanEnd = parentSpan.Position.Absolute + parentSpan.Length;

        if (!token.IndexedPropertyCaptures.Any())
            return [new SpanTwig(token, Path, NestedDepth, OriginalLineText.Substring(token.MatchSpan.Position.Absolute, token.MatchSpan.Length).Replace(Card.ThisToken, CardName))];

        List<NestedSpan> children = [];
        int cursor = parentSpan.Position.Absolute;

        foreach (var indexedProp in token.IndexedPropertyCaptures)
        {
            // 1. Add a twig for any text between the last capture and this one.
            if (indexedProp.Start > cursor)
            {
                var snippetStart = cursor - parentSpan.Position.Absolute;
                var snippetLength = indexedProp.Start - cursor;
                var precedingText = OriginalLineText.Substring(snippetStart + parentSpan.Position.Absolute, snippetLength);
                var precedingTextOrig = parentSpan.ToStringValue().Substring(snippetStart, snippetLength);

                if (!string.IsNullOrWhiteSpace(precedingText))
                    children.Add(new SpanTwig(token, Path, NestedDepth, precedingText.Trim()));
            }

            // 2. Process the actual property capture.
            if (indexedProp.Value is ManyToken manyToken)
            {
                // Use dynamic to access the 'Items' property, which only exists on the generic subclass.
                dynamic dynamicManyToken = manyToken;
                var items = ((IEnumerable)dynamicManyToken.Items).Cast<TokenUnit>().ToList();
                var innerCursor = indexedProp.Start;

                for (int i = 0; i < items.Count; i++)
                {
                    TokenUnit itemToken = items[i];

                    // Text between items
                    if (itemToken.MatchSpan.Position.Absolute > innerCursor)
                    {
                        var snippetStart = innerCursor - parentSpan.Position.Absolute;
                        var snippetLength = itemToken.MatchSpan.Position.Absolute - innerCursor;
                        var textBetween = OriginalLineText.Substring(snippetStart, snippetLength).Replace(Card.ThisToken, CardName);

                        if (!string.IsNullOrWhiteSpace(textBetween))
                            children.Add(new SpanTwig(token, Path, NestedDepth, textBetween.Trim()));
                    }

                    // The item itself
                    children.Add(new SpanBranch(itemToken, CardName, Path.Dot(itemToken.Type.Name), NestedDepth, OriginalLineText, i));
                    innerCursor = itemToken.MatchSpan.Position.Absolute + itemToken.MatchSpan.Length;
                }

                // There might be text after the last item but before the end of the ManyToken span
                if (indexedProp.End > innerCursor)
                {
                    var snippetStart = innerCursor - parentSpan.Position.Absolute;
                    var snippetLength = indexedProp.End - innerCursor;
                    var textAfter = OriginalLineText.Substring(snippetStart, snippetLength);

                    if (!string.IsNullOrWhiteSpace(textAfter))
                        children.Add(new SpanTwig(token, Path, NestedDepth, textAfter.Trim()));
                }
            }
            else if (indexedProp.Value is TokenUnit childToken)
                children.Add(new SpanBranch(childToken, CardName, Path.Dot(childToken.Type.Name), NestedDepth, OriginalLineText));
            else
                children.Add(new SpanLeaf(indexedProp, Path.Dot(indexedProp.RegexPropInfo.Name), NestedDepth, OriginalLineText, CardName));

            // 3. Advance cursor to the end of the current capture.
            cursor = indexedProp.End;
        }

        // 4. Add a final twig for any trailing text after the last capture.
        if (cursor < parentSpanEnd)
        {
            var snippetStart = cursor - parentSpan.Position.Absolute;
            var snippetLength = parentSpanEnd - cursor;
            var trailingText = parentSpan.ToStringValue().Substring(snippetStart, snippetLength).Replace(Card.ThisToken, CardName);
            if (!string.IsNullOrWhiteSpace(trailingText))
            {
                children.Add(new SpanTwig(token, Path, NestedDepth, trailingText.Trim()));
            }
        }

        return children;
    }

    void SetLeavesOrDistilled(TokenUnit token)
    {
        var generatedLeaves = new List<SpanLeaf>();

        // Iterate over the source properties of the token to generate leaves for the property table.
        foreach (var indexedProp in token.IndexedPropertyCaptures)
        {
            if (indexedProp.Value is TokenUnit)
            {
                // This is a branch (nested table), not a leaf for the current table. Skip it.
                continue;
            }

            if (indexedProp.Value is ManyToken manyToken)
            {
                // For a ManyToken, create a synthetic leaf for its Conjunction property to display in the parent's table.
                var conjunctionPropInfo = new RegexPropInfo(typeof(ManyToken).GetProperty(nameof(ManyToken.Conjunction)));
                var conjunctionCapture = new IndexedPropertyCapture(
                    regexPropInfo: conjunctionPropInfo,
                    span: indexedProp.Span, // Use the span of the whole ManyToken capture
                    value: manyToken.Conjunction,
                    capturePosition: indexedProp.CapturePosition // Use the same position for color coding
                );

                generatedLeaves.Add(new SpanLeaf(
                    PropertyCapture: conjunctionCapture,
                    Path: Path.Dot(indexedProp.RegexPropInfo.Name).Dot(nameof(Conjunction)),
                    NestedDepth: NestedDepth + 1,
                    OriginalLineText,
                    CardName
                ));
            }
            else
            {
                // This is a regular scalar property. Create a leaf for it.
                generatedLeaves.Add(new SpanLeaf(
                    PropertyCapture: indexedProp,
                    Path: Path.Dot(indexedProp.RegexPropInfo.Name),
                    NestedDepth: NestedDepth + 1,
                    OriginalLineText,
                    CardName
                ));
            }
        }

        // Now handle the distillation logic, which replaces placeholder leaves with their distilled values.
        if (token is TokenUnitDistilled tokenUnitDistilled)
        {
            var distilledLeaves = new List<SpanLeaf>();
            foreach (var leaf in generatedLeaves)
            {
                // Check if this leaf's property is a placeholder that needs to be replaced.
                if (tokenUnitDistilled.DistilledValues.TryGetValue(leaf.PropertyCapture.RegexPropInfo, out var distilledPropVals))
                {
                    // It is a placeholder. Replace it with its distilled children.
                    foreach (var distilledPropVal in distilledPropVals)
                    {
                        var newPropCapture = leaf.PropertyCapture with { RegexPropInfo = distilledPropVal.Key, Value = distilledPropVal.Value };
                        distilledLeaves.Add(leaf with { PropertyCapture = newPropCapture });
                    }
                }
                else
                {
                    // It's not a placeholder, so keep it.
                    distilledLeaves.Add(leaf);
                }
            }
            LeavesOrDistilled = distilledLeaves;
        }
        else
        {
            LeavesOrDistilled = generatedLeaves;
        }
    }

    public override string ToString() => TokenSpan.ToStringValue();
}