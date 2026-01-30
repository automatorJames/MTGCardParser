using MTGPlexer.RegexGeneration.GraphNodes;

namespace MTGPlexer.TokenUnitGraphComponents;

public abstract class TokenUnitOneOf : TokenUnit
{
    public override string ValidateStructure()
    {
        var rootNode = TokenTypeRegistry.RootNodes[Type];
        var props = GetType().GetProps();

        if (props.Count() < 2) 
            return $"{nameof(TokenUnitOneOf)} must be at least two properties";

        // The poperties as referenced in the constructor should be contiguous
        // (i.e. not separated by text segments)
        bool textSegmentEncountered = false;
        bool capturePropEncountered = false;

        foreach (var node in rootNode.Children)
        {
            // Ignore leading text segments
            if (!capturePropEncountered && node is TextNode)
                continue;

            if (capturePropEncountered && textSegmentEncountered && node is CaptureNode)
                // We've already encountered both captures & non-leading text, so this capture is non-contiguous
                return $"{nameof(TokenUnitOneOf)} properties appear contiguously in base constructor";

            if (node is CaptureNode)
                capturePropEncountered = true;
            else if (node is TextNode)
                textSegmentEncountered = true;
        }

        // Any enums must be nullable
        if (rootNode.Children.OfType<EnumNode>().Any(x => Nullable.GetUnderlyingType(x.PropertySnippet.Prop.PropertyType) == null))
            return $"All enum properties in {nameof(TokenUnitOneOf)} types must be nullable";

        return null;
    }

    public static string GetTokenUnitOneOfRegexHeaderComment(Type tokenUnitOneOfType)
    {
        var tokenUnitChildPropNames = tokenUnitOneOfType.GetProps().Select(x => x.Name);
        return $"(?# {tokenUnitOneOfType.Name}: {string.Join(" | ", tokenUnitChildPropNames)})";
    }
}