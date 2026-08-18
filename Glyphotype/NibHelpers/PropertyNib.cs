namespace Glyphotype.NibHelpers;

public record PropertyNib : Nib
{
    public PropertyInfo Prop { get; }
    public Type Type { get; }
    public string Name { get; }
    public Proptions Proptions { get; }
    public Quantifier? Quantifier { get; }

    public PropertyNib(string text, PropertyInfo prop, Proptions proptions, Quantifier? quantifier = null) 
        : base(text)
    {
        Prop = prop;
        Proptions = proptions;
        Type = prop.PropertyType;
        Name = prop.Name;
        Quantifier = quantifier;

        // Extract metadata info from property attributes
        // Todo: we should be using Quantifier to express quantifiers, not Proptions

        if (Prop.IsDefined(typeof(OneOrMoreAttribute)))
            Proptions |= Proptions.OneOrMore;

        if (Prop.IsDefined(typeof(OptionalAttribute)))
            Proptions |= Proptions.Optional;
    }

    public static PropertyNib[] GetPropertyNibs(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => x.GetSetMethod() != null) // Ignore get-only props like Joiner overrides
            .Select(x => new PropertyNib(x.Name, x, Proptions.None))
            .ToArray();
}