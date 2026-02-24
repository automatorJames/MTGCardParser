namespace MTGPlexer.TokenUnitPrimitives;

public class TokenUnitFused : TokenUnit
{
    public override Joiner Joiner => Joiner.None;
    public virtual Snippet BeforeContent => null;
    public virtual Snippet AfterContent => null;

    public override string ValidateStructure()
    {
        var graph = TokenTypeRegistry.RegexGraphs[Type];
        var props = GetType().GetProps();

        if (!props.Any())
            return $"Snippets for {Type.Name} must contain at least one property reference";

        if (!graph.RootNode.ValidateCapturePropertiesAreContiguous())
            return $"Snippets for {Type.Name} contains more than one contiguous run of property references interspersed by text";

        // todo: we should ponder whether it makes sense for this type to support any property types other nullable ints
        if (!props.All(x => x.PropertyType == typeof(int?)))
            return $"{Type.Name} must contain only nullable int properties";

        return base.ValidateStructure();
    }
}