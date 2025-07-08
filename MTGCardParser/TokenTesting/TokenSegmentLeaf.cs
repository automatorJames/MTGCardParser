namespace MTGCardParser.TokenTesting;

/// <summary>
/// Represents a content leaf whose text has been fully segmented into
/// plain text and property captures, ready for rendering.
/// </summary>
public record TokenSegmentLeaf : TokenSegment
{
    public List<LeafPart> Parts { get; }

    // The signature now accepts our clean, new type.
    public TokenSegmentLeaf(string leafText, int leafAbsoluteStart, List<IndexedPropertyCapture> orderedParentCaptures)
    {
        var parts = new List<LeafPart>();
        if (string.IsNullOrEmpty(leafText))
        {
            Parts = parts;
            return;
        }

        // The captures are already ordered, so we just need to filter them.
        var relevantCaptures = orderedParentCaptures
            .Where(c => c.Span.Position.Absolute >= leafAbsoluteStart &&
                        (c.Span.Position.Absolute + c.Span.Length) <= (leafAbsoluteStart + leafText.Length))
            .ToList();

        if (!relevantCaptures.Any())
        {
            parts.Add(new PlainTextPart(leafText));
            Parts = parts;
            return;
        }

        // The rest of the logic is now much cleaner to read as it operates
        // on the self-documenting 'IndexedPropertyCapture' record.
        int currentIndexInLeaf = 0;
        foreach (var capture in relevantCaptures)
        {
            int propRelativeStart = capture.Span.Position.Absolute - leafAbsoluteStart;

            if (propRelativeStart > currentIndexInLeaf)
            {
                parts.Add(new PlainTextPart(leafText.Substring(currentIndexInLeaf, propRelativeStart - currentIndexInLeaf)));
            }

            parts.Add(new PropertyCapturePart(capture.Span.ToStringValue(), capture.Property, capture.OriginalIndex));

            currentIndexInLeaf = propRelativeStart + capture.Span.Length;
        }

        if (currentIndexInLeaf < leafText.Length)
        {
            parts.Add(new PlainTextPart(leafText.Substring(currentIndexInLeaf)));
        }

        Parts = parts;
    }
}