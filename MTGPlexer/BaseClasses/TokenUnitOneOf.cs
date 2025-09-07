namespace MTGPlexer.BaseClasses;

public abstract class TokenUnitOneOf: TokenUnit
{
    protected TokenUnitOneOf(params string[] templateSnippets) : base(templateSnippets)
    {
    }

    /// <summary>
    /// Returns a single non-null child TokenUnit which represents the "one" property with a value among 
    /// this instance's canddiate values. As an analytical precaution, an exception is thrown if no
    /// non-null TokenUnit is found.
    /// </summary>
    public TokenUnit GetSingleNonNullChildToken()
    {
        foreach (var prop in GetType().GetProps())
        {
            var propVal = prop.GetValue(this);

            if (propVal is TokenUnit tokenUnit)
                return tokenUnit;
        }

        throw new Exception("Expected a non-null TokenUnit child property, but found none");
    }

    public override bool ValidateStructure()
    {
        var props = GetType().GetProps();

        // There should be at least two properties
        if (props.Count() < 2) 
            return false;

        // The poperties as referenced in the constructor should be contiguous
        // (i.e. not separated by text segments)
        bool textSegmentEncountered = false;
        bool capturePropEncountered = false;
        foreach (var segment in Template.RegexSegments)
        {
            // Ignore leading text segments
            if (!capturePropEncountered && segment is TextSegment)
                continue;

            if (capturePropEncountered && textSegmentEncountered && segment is CaptureGroupPropBase)
                // We've already encountered both captures & non-leading text, so this capture is non-contiguous
                return false;

            if (segment is CaptureGroupPropBase)
                capturePropEncountered = true;
            else if (segment is TextSegment)
                textSegmentEncountered = true;
        }

        return true;
    }

    public static string GetTokenUnitOneOfRegexHeaderComment(Type tokenUnitOneOfType)
    {
        var tokenUnitChildPropNames = tokenUnitOneOfType.GetProps().Select(x => x.Name);
        return $"(?# {tokenUnitOneOfType.Name}: {string.Join(" | ", tokenUnitChildPropNames)})";
    }
}