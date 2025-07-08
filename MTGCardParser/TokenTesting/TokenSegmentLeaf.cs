namespace MTGCardParser.TokenTesting;

/// <summary>
/// Represents a content leaf whose text has been fully segmented into
/// plain text and property captures, ready for rendering.
/// </summary>
public record TokenSegmentLeaf : TokenSegment
{
    public IReadOnlyList<LeafPart> Parts { get; }

    public TokenSegmentLeaf(string leafText, int leafAbsoluteStart, IReadOnlyList<KeyValuePair<CaptureProp, TextSpan>> parentPropMatches)
    {
        var parts = new List<LeafPart>();
        if (string.IsNullOrEmpty(leafText))
        {
            Parts = parts;
            return;
        }

        // 1. Filter and sort properties relevant to THIS leaf.
        var relevantCaptures = parentPropMatches
            .Select((kvp, index) => new { Prop = kvp.Key, Span = kvp.Value, OriginalIndex = index })
            .Where(x => x.Span.Position.Absolute >= leafAbsoluteStart && (x.Span.Position.Absolute + x.Span.Length) <= (leafAbsoluteStart + leafText.Length))
            .OrderBy(x => x.Span.Position.Absolute)
            .ToList();

        if (!relevantCaptures.Any())
        {
            parts.Add(new PlainTextPart(leafText));
            Parts = parts;
            return;
        }

        // 2. Build the sequence of LeafParts.
        int currentIndexInLeaf = 0;
        foreach (var capture in relevantCaptures)
        {
            int propRelativeStart = capture.Span.Position.Absolute - leafAbsoluteStart;

            if (propRelativeStart > currentIndexInLeaf)
            {
                parts.Add(new PlainTextPart(leafText.Substring(currentIndexInLeaf, propRelativeStart - currentIndexInLeaf)));
            }
            
            parts.Add(new PropertyCapturePart(capture.Span.ToStringValue(), capture.Prop, capture.OriginalIndex));
            
            currentIndexInLeaf = propRelativeStart + capture.Span.Length;
        }

        // 3. Render any remaining text.
        if (currentIndexInLeaf < leafText.Length)
        {
            parts.Add(new PlainTextPart(leafText.Substring(currentIndexInLeaf)));
        }

        Parts = parts;
    }
}