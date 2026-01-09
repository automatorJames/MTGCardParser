namespace MTGPlexer.TokenUnitComponents;

public abstract class TokenUnitOneOf : TokenUnit
{
    /// <summary>
    /// Returns a single non-null child value which represents the "one" property with a value among 
    /// this instance's canddiate values. As an analytical precaution, an exception is thrown if not exactly
    /// one non-null IndexedPropertyCapture is found.
    /// </summary>
    public PropertyCapture GetIndexedPropertyCaptureSingle()
    {
        if (IndexedPropertyCaptures.Count != 1)
            throw new Exception($"Expected a single {nameof(PropertyCapture)}, but found {IndexedPropertyCaptures.Count}");

        return IndexedPropertyCaptures.First();
    }

    public override string ValidateStructure()
    {
        var template = TokenTypeRegistry.Templates[Type];
        var props = GetType().GetProps();

        if (props.Count() < 2) 
            return $"{nameof(TokenUnitOneOf)} must be at least two properties";

        // The poperties as referenced in the constructor should be contiguous
        // (i.e. not separated by text segments)
        bool textSegmentEncountered = false;
        bool capturePropEncountered = false;
        foreach (var segment in template.RegexSegments)
        {
            // Ignore leading text segments
            if (!capturePropEncountered && segment is TextSegment)
                continue;

            if (capturePropEncountered && textSegmentEncountered && segment is CaptureGroupPropBase)
                // We've already encountered both captures & non-leading text, so this capture is non-contiguous
                return $"{nameof(TokenUnitOneOf)} properties appear contiguously in base constructor";

            if (segment is CaptureGroupPropBase)
                capturePropEncountered = true;
            else if (segment is TextSegment)
                textSegmentEncountered = true;
        }

        return null;
    }

    public static string GetTokenUnitOneOfRegexHeaderComment(Type tokenUnitOneOfType)
    {
        var tokenUnitChildPropNames = tokenUnitOneOfType.GetProps().Select(x => x.Name);
        return $"(?# {tokenUnitOneOfType.Name}: {string.Join(" | ", tokenUnitChildPropNames)})";
    }
}