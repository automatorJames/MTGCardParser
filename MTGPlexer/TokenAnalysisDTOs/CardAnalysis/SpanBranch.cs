namespace MTGPlexer.TokenAnalysisDTOs.CardAnalysis;

public record SpanBranch : NestedSpan
{
    public string CardName { get; set; }
    public string DisplayName { get; set; }
    public List<NestedSpan> Children { get; }
    public List<SpanBranch> Branches { get; }
    public List<SpanLeaf> Leaves { get; }
    public List<SpanLeaf> LeavesOrDistilled { get; private set; } = [];
    public Capture Capture { get; }
    public Type TokenType { get; }
    public bool CollapseInAnalysis { get; }
    public string OriginalLineText { get; }
    public int? ManyIndex { get; }
    public string Text => Capture.Value.Trim();

    public SpanBranch(TokenUnit token, string cardName, string parentPath, int parentDepth, string originalLineText, int? manyIndex = null)
        : base(
            Path: parentPath.Dot(token.Capture.ToIndexString()).Dot(token.Type.Name),
            NestedDepth: parentDepth + 1,
            Palette: TokenTypeRegistry.Palettes[token.Type],
            IgnoreInAnalysis: token.Type.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null)
    {
        OriginalLineText = originalLineText;
        CardName = cardName;
        Children = DigestChildren(token);
        Branches = Children.OfType<SpanBranch>().ToList();
        Leaves = Children.OfType<SpanLeaf>().ToList();
        SetLeavesOrDistilled(token);
        Capture = token.Capture;
        TokenType = token.Type;
        ManyIndex = manyIndex;
        DisplayName = token.Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);

        CollapseInAnalysis = token is TokenUnitOneOf tokenUnitOneOf && tokenUnitOneOf.GetChildTokens().Any(x => x.Type.IsAssignableTo(typeof(TokenUnit)));

        if (token.ParentToken is TokenUnitOneOf parentTokenUnitOneOf)
            DisplayName = $"{parentTokenUnitOneOf.Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence)}: {DisplayName}";
    }

    private List<NestedSpan> DigestChildren(TokenUnit token)
    {
        var capture = token.Capture;
        var matchEnd = capture.Index + capture.Length;

        if (!token.IndexedPropertyCaptures.Any())
            return [new SpanTwig(token, Path, NestedDepth, OriginalLineText.Substring(capture.Index, capture.Length).Replace(Card.ThisToken, CardName))];

        List<NestedSpan> children = [];
        int cursor = capture.Index;

        foreach (var indexedProp in token.IndexedPropertyCaptures)
        {
            // 1. Add a twig for any text between the last capture and this one.
            if (indexedProp.Start > cursor)
            {
                var snippetStart = cursor - capture.Index;
                var snippetLength = indexedProp.Start - cursor;
                var precedingText = OriginalLineText.Substring(snippetStart + capture.Index, snippetLength);
                var precedingTextOrig = capture.Value.Substring(snippetStart, snippetLength);

                if (!string.IsNullOrWhiteSpace(precedingText))
                    children.Add(new SpanTwig(token, Path, NestedDepth, precedingText.Trim()));
            }

            // 2. Process the actual property capture.
            if (indexedProp.Value is ManyOf manyToken)
            {
                var items = manyToken.ItemObjects;
                var allCaptures = new List<Capture>();

                // Create a unified list of all items and the conjunction to process them in order.
                allCaptures.AddRange(items.Select(i => i.Capture));

                if (manyToken.Conjunction.HasValue)
                    allCaptures.Add(manyToken.ConjunctionCapture);

                // Sort all captures by their starting index to process them in the order they appear.
                var sortedCaptures = allCaptures.OrderBy(c => c.Index).ToList();

                // Keep track of how many many items we've encountered so we can set their data path properly
                int manyItemCurrentIndex = 0;

                var innerCursor = indexedProp.Start;

                for (int i = 0; i < sortedCaptures.Count; i++)
                {
                    Capture currentCapture = sortedCaptures[i];
                    // Text between the last capture and the current one.
                    if (currentCapture.Index > innerCursor)
                    {
                        var snippetStart = innerCursor - capture.Index;
                        var snippetLength = currentCapture.Index - innerCursor;
                        var textBetween = OriginalLineText.Substring(snippetStart, snippetLength).Replace(Card.ThisToken, CardName);

                        if (!string.IsNullOrWhiteSpace(textBetween))
                            children.Add(new SpanTwig(token, Path, NestedDepth, textBetween.Trim()));
                    }

                    // Check if the current capture is the conjunction.
                    if (manyToken.Conjunction.HasValue && currentCapture.Index == manyToken.ConjunctionCapture.Index)
                    {
                        var prop = manyToken.GetType().GetProperty(nameof(ManyOf.Conjunction));
                        RegexPropInfo propInfo = new(prop);
                        IndexedPropertyCapture derivedIndexProp = new(propInfo, manyToken.ConjunctionCapture, manyToken.Conjunction, i);
                        var path = Path.Dot(indexedProp.RegexPropInfo.Name).Dot(prop.Name);
                        children.Add(new SpanLeaf(derivedIndexProp, path, NestedDepth, OriginalLineText, CardName));
                    }
                    else // Otherwise, it's a regular many-item.
                    {
                        var manyItem = items.First(i => i.Capture.Index == currentCapture.Index);
                        int itemIndex = items.IndexOf(manyItem);
                        var path = Path.Dot(indexedProp.RegexPropInfo.Name) + $"_item{++manyItemCurrentIndex}";

                        if (manyToken.ManyItemType == ManyItemType.TokenUnit && manyItem.ItemAsObject is TokenUnit itemToken)
                            children.Add(new SpanBranch(itemToken, CardName, path, NestedDepth, OriginalLineText, itemIndex));
                        else if (manyToken.ManyItemType == ManyItemType.Enum)
                        {
                            IndexedPropertyCapture derivedIndexProp = new(indexedProp.RegexPropInfo, manyItem.Capture, manyItem.Capture.Value, i);
                            children.Add(new SpanLeaf(derivedIndexProp, path, NestedDepth, OriginalLineText, CardName));
                        }
                    }

                    innerCursor = currentCapture.Index + currentCapture.Length;
                }

                // There might be text after the last item but before the end of the ManyToken span
                if (indexedProp.End > innerCursor)
                {
                    var snippetStart = innerCursor - capture.Index;
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
        if (cursor < matchEnd)
        {
            var snippetStart = cursor - capture.Index;
            var snippetLength = matchEnd - cursor;
            var trailingText = capture.Value.Substring(snippetStart, snippetLength).Replace(Card.ThisToken, CardName);
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

            if (indexedProp.Value is ManyOf manyToken)
            {
                // For a ManyToken, create a synthetic leaf for its Conjunction property to display in the parent's table (if not null)
                if (manyToken.Conjunction != null)
                {
                    var conjunctionPropInfo = new RegexPropInfo(typeof(ManyOf).GetProperty(nameof(ManyOf.Conjunction)));

                    var conjunctionCapture = new IndexedPropertyCapture(
                        regexPropInfo: conjunctionPropInfo,
                        capture: manyToken.ConjunctionCapture,
                        value: manyToken.Conjunction,
                        capturePosition: 0
                    );

                    generatedLeaves.Add(new SpanLeaf(
                        PropertyCapture: conjunctionCapture,
                        Path: Path.Dot(indexedProp.RegexPropInfo.Name).Dot(nameof(Conjunction)),
                        NestedDepth: NestedDepth + 1,
                        OriginalLineText: OriginalLineText,
                        CardName: CardName
                    ));
                }

                for (int i = 0; i < manyToken.ItemObjects.Count; i++)
                {
                    var item = manyToken.ItemObjects[i];
                    IndexedPropertyCapture itemIndexPropertyCapture = new(indexedProp.RegexPropInfo, item.Capture, item.ItemAsObject, i + 1);

                    generatedLeaves.Add(new SpanLeaf(
                        itemIndexPropertyCapture,
                        Path.Dot($"{indexedProp.RegexPropInfo.Name}_item{i + 1}"),
                        NestedDepth + 1,
                        OriginalLineText,
                        CardName
                        ));
                }
            }
            else
            {
                // This is a regular scalar property. Create a leaf for it.
                generatedLeaves.Add(new SpanLeaf(
                    PropertyCapture: indexedProp,
                    Path: Path.Dot(indexedProp.RegexPropInfo.Name),
                    NestedDepth: NestedDepth + 1,
                    OriginalLineText: OriginalLineText,
                    CardName: CardName
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

    public override string ToString() => Capture.Value;
}