namespace Glyphotype.RegexGeneration.Graph;

public class Navigation
{
    public string Name { get; private set; }
    public Type Type { get; private set; }
    public Type UnderlyingType { get; private set; }
    public Type[] GenericTypes { get; private set; }
    public Type NodeType { get; private set; }
    public string[] Patterns { get; private set; }

    // Only used for navigations to properties
    public PropertyInfo Prop { get; private set; }
    public Proptions Proptions { get; private set; } = Proptions.None;
    public Quantifier? Quantifier { get; private set; }

    // Only used for navigations to Glyph types
    public GlyphTypeConfiguration GlyphTypeConfiguration { get; private set; }

    // Convenience bools to simplify logic in Node constructors
    public bool IsCaptureUnitType { get; private set; }
    public bool IsRoot { get; private set; }
    public bool IsList { get; private set; }
    public bool IsOptional { get; private set; }

    public Navigation(Type type)
    {
        SetTypeInfo(type);

        if (!IsCaptureUnitType)
            throw new Exception($"This constructor may only be used for {nameof(CaptureUnit)} types");

        IsRoot = true;
        Name = GetRegexSafeTypeName(UnderlyingType);
        Patterns = Type.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
    }

    /// <summary>
    /// A regex-group-name-safe rendering of <paramref name="type"/>'s name: plain for an ordinary type, but
    /// for a closed generic (e.g. <c>OneOf&lt;CardType, CreatureType&gt;</c>, whose raw <see cref="Type.Name"/>
    /// is "OneOf`2") strips the backtick-arity suffix and appends each type argument's own safe name (e.g.
    /// "OneOfCardTypeCreatureType"), since the backtick and any punctuation from a friendly display name
    /// would otherwise produce an invalid .NET regex named-capture-group identifier.
    /// </summary>
    static string GetRegexSafeTypeName(Type type) =>
        !type.IsGenericType
            ? type.Name
            : type.Name[..type.Name.IndexOf('`')] + string.Concat(type.GetGenericArguments().Select(GetRegexSafeTypeName));

    public Navigation(PropertyNib propertyNib)
    {
        SetTypeInfo(propertyNib.Type);
        Name = propertyNib.Name;
        Patterns = propertyNib.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
        Prop = propertyNib.Prop;
        Proptions = propertyNib.Proptions;
        IsOptional = Prop.IsDefined(typeof(OptionalAttribute));
    }

    void SetTypeInfo(Type type)
    {
        Type = type;
        UnderlyingType = (Nullable.GetUnderlyingType(type) ?? type);
        GenericTypes = UnderlyingType.GenericTypeArguments;
        IsList = UnderlyingType.IsGenericType && UnderlyingType.GetGenericTypeDefinition() == typeof(List<>);
        NodeType = IsList ? GenericTypes[0] : UnderlyingType;
        IsCaptureUnitType = NodeType.IsAssignableTo(typeof(CaptureUnit));

        if (IsCaptureUnitType)
            GlyphTypeConfiguration = GlyphTypeRegistry.GetGlyphTypeConfiguration(NodeType);
    }

    public override string ToString() => Name;
}
