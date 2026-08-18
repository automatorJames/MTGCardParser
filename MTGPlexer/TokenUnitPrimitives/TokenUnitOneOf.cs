namespace MTGPlexer.TokenUnitPrimitives;

public abstract class TokenUnitOneOf : OneOfBase
{
    public override string ValidateStructure()
    {
        var graph = TokenTypeRegistry.RegexGraphs[Type];
        var props = GetType().GetProps();

        if (props.Count() < 2) 
            return $"Snippets for {Type.Name} must contain at least two property references";

        if (!graph.RootNode.ValidateCapturePropertiesAreContiguous())
            return $"Snippets for {Type.Name} contains more than one contiguous run of property references interspersed by text";

        // Any enums must be nullable
        if (graph.RootNode.Children.OfType<EnumNode>().Any(x => Nullable.GetUnderlyingType(x.Navigation.Type) == null))
            return $"All enum properties in {nameof(TokenUnitOneOf)} types must be nullable";

        return base.ValidateStructure();
    }
}