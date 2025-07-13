namespace MTGPlexer.TokenAnalysis.DTOs;

/// <summary>
/// Represents a content leaf whose text has been fully segmented into
/// plain text and property captures, ready for rendering.
/// </summary>
public record TokenSegmentLeaf : TokenSegment
{
    public List<TokenLeafPart> Parts { get; private set; }
    public bool IsComplexToken { get; }
    public string ParentPath { get; }

    public TokenSegmentLeaf(string leafText, int leafAbsoluteStart, TokenUnit token, string parentPath)
    {
        IsComplexToken = token is TokenUnitComplex;
        ParentPath = parentPath;
        Decompose(leafText, leafAbsoluteStart, token);
    }

    void Decompose(string leafText, int leafAbsoluteStart, TokenUnit token)
    {
        var parts = new List<TokenLeafPart>();
        if (string.IsNullOrEmpty(leafText))
        {
            Parts = parts;
            return;
        }

        var leafAbsoluteEnd = leafAbsoluteStart + leafText.Length;

        // Get the property captures whose spans are contained within this leaf's span
        var relevantPropertyCaptures = token.IndexedPropertyCaptures
            .Where(x => x.Start >= leafAbsoluteStart && x.End <= leafAbsoluteEnd)
            .ToList();

        // If no properties fall within this leaf, it will be rendered as non-property leaf part
        // This means it will have underlines signifying its part of one or more tokens, but it won't have an overline
        if (!relevantPropertyCaptures.Any())
        {
            parts.Add(new NonPropertyTokenLeafPart(leafText));
            Parts = parts;
        }
        else
        {
            int currentIndexInLeaf = 0;

            for (int i = 0; i < relevantPropertyCaptures.Count; i++)
            {
                var capture = relevantPropertyCaptures[i];
                int propRelativeStart = capture.Start - leafAbsoluteStart;

                if (propRelativeStart > currentIndexInLeaf)
                {
                    // There is some text left of the captured property value that's not part of the property itself, so encapsulate it here
                    // This part of the text will not have an overline
                    var precedingTextSnippet = leafText.Substring(currentIndexInLeaf, propRelativeStart - currentIndexInLeaf);
                    parts.Add(new NonPropertyTokenLeafPart(precedingTextSnippet));
                }

                // Add the captured property itself
                // It will be displayed with an overline
                parts.Add(new PropertyCaptureTokenLeafPart(capture.Span.ToStringValue(), capture.RegexPropInfo, capture.Index, ParentPath));
                currentIndexInLeaf = propRelativeStart + capture.Span.Length;
            }

            if (currentIndexInLeaf < leafText.Length)
            {
                // There is some text right of the captured property value that's not part of the property itself, so encapsulate it here
                // This part of the text will not have an overline
                var followingTextSnippet = leafText.Substring(currentIndexInLeaf);
                parts.Add(new NonPropertyTokenLeafPart(followingTextSnippet));
            }

            Parts = parts;
        }
    }

    public override string ToString() => string.Join("", Parts.Select(x => x.Text)).Trim();
}