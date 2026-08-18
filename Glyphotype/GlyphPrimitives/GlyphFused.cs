namespace Glyphotype.GlyphPrimitives;

public class GlyphFused<T> : Glyph where T : Glyph
{
    public override Nib[] Nibs => GetNibs();

    Nib[] GetNibs()
    {
        var fusedPropertyNib = Prop(FusedContent, quantifier: Quantifier.OneOrMore);

        return (BeforeContent, AfterContent) switch
        {
            (null, null) =>         [fusedPropertyNib],
            (not null, null) =>     [BeforeContent, fusedPropertyNib],
            (null, not null) =>     [fusedPropertyNib, AfterContent],
            (not null, not null) => [BeforeContent, fusedPropertyNib, AfterContent],
        };
    }

    public virtual Nib BeforeContent => null;
    public virtual Nib AfterContent => null;
    public override Joiner Joiner => Joiner.None;

    public T FusedContent { get; set; }

    public override string ValidateStructure()
    {
        var graph = GlyphTypeRegistry.RegexGraphs[Type];
        var props = GetType().GetProps();

        if (!props.Any())
            return $"Nibs for {Type.Name} must contain at least one property reference";

        if (!graph.RootNode.ValidateCapturePropertiesAreContiguous())
            return $"Nibs for {Type.Name} contains more than one contiguous run of property references interspersed by text";

        // todo: we should ponder whether it makes sense for this type to support any property types other nullable ints
        if (!props.All(x => x.PropertyType == typeof(int?)))
            return $"{Type.Name} must contain only nullable int properties";

        return base.ValidateStructure();
    }
}