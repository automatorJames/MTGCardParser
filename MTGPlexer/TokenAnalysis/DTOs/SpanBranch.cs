namespace MTGPlexer.TokenAnalysis.DTOs;

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
    public string Text => TokenSpan.ToStringValue().Trim();

    public SpanBranch(TokenUnit token, string cardName, string parentPath, int parentDepth) 
        : base(
            Path: parentPath.Dot(token.MatchSpan.IndexString()).Dot(token.Type.Name),
            NestedDepth: parentDepth + 1, 
            Palette: TokenTypeRegistry.Palettes[token.Type], 
            IgnoreInAnalysis: token.Type.GetCustomAttribute<IgnoreInAnalysisAttribute>() != null)
    {
        CardName = cardName;
        DisplayName = token.Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        Children = DigestChildren(token);
        Branches = Children.OfType<SpanBranch>().ToList();
        Leaves = Children.OfType<SpanLeaf>().ToList();
        SetLeavesOrDistilled(token);
        TokenSpan = token.MatchSpan;
        TokenType = token.Type;
        CollapseInAnalysis = token is TokenUnitOneOf;
    }

    List<NestedSpan> DigestChildren(TokenUnit token)
    {
        var parentSpan = token.MatchSpan;
        var parentSpanEnd = parentSpan.Position.Absolute + parentSpan.Length;

        // If there are no children, this is a leaf node.
        // We treat its entire content as a single TextUnit.
        if (!token.IndexedPropertyCaptures.Any())
            return [new SpanTwig(token, Path, NestedDepth, token.MatchSpan.ToStringValue().Trim())];

        // If there are children, dissect the parent's text.
        List<NestedSpan> children = [];

        // Cursor to follow current absolute position within parent span
        int cursor = parentSpan.Position.Absolute;

        foreach (var indexedProp in token.IndexedPropertyCaptures)
        {
            // Create a preceding snippet for any text between the cursor and the start of this child (i.e.
            // plain text from the parent that will display occur preceding this child)
            if (indexedProp.Start > cursor)
            {
                var snippetStart = cursor - parentSpan.Position.Absolute;
                var snippetLength = indexedProp.Start - cursor;
                var precedingText = parentSpan.ToStringValue().Substring(snippetStart, snippetLength);

                if (precedingText != " ")
                    children.Add(new SpanTwig(token, Path, NestedDepth, precedingText.Trim()));

                cursor += precedingText.Length;
            }

            // Child TokenUnits get digested recursively
            if (indexedProp.Value is TokenUnit childToken)
            {
                SpanBranch branch = new SpanBranch(childToken, CardName, Path.Dot(childToken.Type.Name), NestedDepth);
                children.Add(branch);
                cursor += childToken.MatchSpan.Length;
            }

            // Other types of prop captures are rendered into leaves
            else
            {
                var leaf = new SpanLeaf(indexedProp, Path.Dot(indexedProp.RegexPropInfo.Name), NestedDepth);
                children.Add(leaf);
                cursor += indexedProp.Length;
            }
        }

        // Create a final span for any trailing text after the last child.
        if (cursor < parentSpanEnd)
        {
            var snippetStart = cursor - parentSpan.Position.Absolute;
            var snippetLength = parentSpanEnd - cursor;
            var followingText = parentSpan.ToStringValue().Substring(snippetStart, snippetLength);
            var followingTwig = new SpanTwig(token, Path, NestedDepth, followingText.Trim());
            children.Add(followingTwig);
        }

        //if (token.MatchSpan.ToStringValue() == "({t}: add {b} or {r}.)") Debugger.Break();
        return children;
    }

    void SetLeavesOrDistilled(TokenUnit token)
    {
        if (token is TokenUnitDistilled tokenUnitDistilled)
        {
            List<SpanLeaf> distilledLeaves = [];
            foreach (var leaf in Leaves)
            {
                var distilledPropVals = tokenUnitDistilled.DistilledValues[leaf.PropertyCapture.RegexPropInfo.Prop];

                foreach (var propVal in distilledPropVals)
                {
                    var newRegexPropInfo = new RegexPropInfo(propVal.Key);
                    var newPropCapture = leaf.PropertyCapture with { RegexPropInfo = newRegexPropInfo, Value = propVal.Value };
                    var distilledLeaf = leaf with { PropertyCapture = newPropCapture };
                    LeavesOrDistilled.Add(distilledLeaf);
                }
            }
        }
        else
            LeavesOrDistilled = Leaves;
    }

    public override string ToString() => TokenSpan.ToStringValue();
}