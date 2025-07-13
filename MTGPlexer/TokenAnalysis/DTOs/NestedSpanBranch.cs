namespace MTGPlexer.TokenAnalysis.DTOs;

public record NestedSpanBranch : NestedSpan
{
    public string DisplayName { get; set; }
    public List<NestedSpan> Children { get; }
    public List<NestedSpanBranch> Branches { get; }
    public List<NestedSpanLeaf> Leaves { get; }
    public bool IgnoreInAnalysis { get; }
    public TextSpan TokenSpan { get; }

    public NestedSpanBranch(TokenUnit token, string parentPath, int parentDepth) 
        : base(parentPath.Dot(token.Type.Name), parentDepth + 1, TokenTypeRegistry.TypeColorPalettes[token.Type])
    {
        DisplayName = token.Type.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        Children = DigestChildren(token);
        Branches = Children.OfType<NestedSpanBranch>().ToList();
        Leaves = Children.OfType<NestedSpanLeaf>().ToList();
        IgnoreInAnalysis = token.Type.GetCustomAttribute<IgnoreInAnalysisAttribute>() is not null;
        TokenSpan = token.MatchSpan;
    }

    List<NestedSpan> DigestChildren(TokenUnit token)
    {
        //if (token.MatchSpan.ToStringValue() == "enchant creature") Debugger.Break();


        var parentSpan = token.MatchSpan;
        var parentSpanEnd = parentSpan.Position.Absolute + parentSpan.Length;

        // If there are no children, this is a leaf node.
        // We treat its entire content as a single TextUnit.
        if (!token.IndexedPropertyCaptures.Any())
            return [new NestedSpanTwig(token, Path, NestedDepth, token.MatchSpan.ToStringValue())];

        // If there are children, dissect the parent's text.
        List<NestedSpan> children = [];

        // Cursor to follow current absolute position within parent span
        int cursor = parentSpan.Position.Absolute;

        foreach (var indexedProp in token.IndexedPropertyCaptures)
        {
            // Create a TextUnit for any text between the cursor and the start of this child (i.e.
            // plain text from the parent that will display occur preceding this child)
            if (indexedProp.Start > cursor)
            {
                var snippetStart = cursor - parentSpan.Position.Absolute;
                var snippetLength = indexedProp.Start - cursor - 1; // minus 1 to account for space
                var precedingText = parentSpan.ToStringValue().Substring(snippetStart, snippetLength);
                children.Add(new NestedSpanTwig(token, Path, NestedDepth, precedingText));
                cursor += precedingText.Length + 1; // + 1 to account for trailing space
            }

            // Child TokenUnits get digested recursively
            if (indexedProp.Value is TokenUnit childToken)
            {
                NestedSpanBranch branch = new NestedSpanBranch(childToken, Path.Dot(childToken.Type.Name), NestedDepth);
                children.Add(branch);
                cursor += childToken.MatchSpan.Length;
            }

            // Other types of prop captures are rendered into leaves
            else
            {
                var leaf = new NestedSpanLeaf(indexedProp, Path.Dot(indexedProp.RegexPropInfo.Name), NestedDepth);
                children.Add(leaf);
                cursor += indexedProp.Length + 1; // + 1 to account for trailing space
            }

            // If cursor isn't at the end of the parent span, advance it one (space between words)
            if (cursor < parentSpanEnd)
                cursor++;
        }

        // Create a final TextUnit for any trailing text after the last child.
        if (cursor < parentSpanEnd)
        {
            var snippetStart = cursor - parentSpan.Position.Absolute;
            var snippetLength = parentSpanEnd - cursor;
            var followingText = parentSpan.ToStringValue().Substring(snippetStart, snippetLength);
            children.Add(new NestedSpanTwig(token, Path, NestedDepth, followingText));
        }

        return children;
    }
}