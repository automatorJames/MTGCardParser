using MTGPlexer.RegexGeneration.Graph.Nodes;

namespace MTGPlexer.TokenUnitGraphComponents;

public abstract class TokenUnitOneOf : TokenUnit
{
    public override string ValidateStructure()
    {
        var rootNode = TokenTypeRegistry.RootNodes[Type];
        var props = GetType().GetProps();

        if (props.Count() < 2) 
            return $"Snippets for {Type.Name} must contain at least two property references";

        if (!rootNode.ValidateCapturePropertiesAreContiguous())
            return $"Snippets for {Type.Name} contains more than one contiguous run of property references interspersed by text";

        // Any enums must be nullable
        if (rootNode.Children.OfType<EnumNode>().Any(x => Nullable.GetUnderlyingType(x.Navigable.Type) == null))
            return $"All enum properties in {nameof(TokenUnitOneOf)} types must be nullable";

        return base.ValidateStructure();
    }
}