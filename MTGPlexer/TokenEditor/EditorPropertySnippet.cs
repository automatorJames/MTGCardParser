
namespace MTGPlexer.CommonDTOs;

public record EditorPropertySnippet(Type BasePropertyType, XOfType XOfType, string Id) 
    : EditorSnippet(
        EditorRepresentation: "@" + GetTypeRepresentation(BasePropertyType, XOfType),
        ParameterRepresentation: GetParameterRepresentation(BasePropertyType, XOfType),
        DisplayAsBlockInEditor: true,
        Id: Id)
{
    public bool BaseIsEnum { get; } = BasePropertyType.IsEnum;
    public Type ClosedGenericType { get; } = GetClosedGenericType(BasePropertyType, XOfType);
    public string PropertyTypeRepresentation { get; } = GetTypeRepresentation(BasePropertyType, XOfType);
    public string PropertyNameRepresentation { get; } = GetPropertyNameRepresentation(BasePropertyType, XOfType);
    public Type ResolvedType => ClosedGenericType ?? BasePropertyType;

    static string GetTypeRepresentation(Type propertyType, XOfType xOfType)
    {
        var representation = propertyType.Name;

        if (xOfType != XOfType.None)
            representation = $"{xOfType}<{representation}>";

        return representation ;
    }

    static Type GetClosedGenericType(Type basePropertyType, XOfType xOfType)
    {
        if (xOfType == XOfType.None)
            return null;

        var openGenericType = xOfType switch
        {
            XOfType.ManyOf => typeof(ManyOf<>),
            XOfType.CompoundOf => typeof(CompoundOf<>),
            XOfType.OptionalOf => typeof(OptionalOf<>),
            XOfType.DynamicOf => throw new NotImplementedException($"DynamicOf not yet supported as editor snippet"),
            _ => throw new ArgumentException($"Unsupported type: '{xOfType}'")
        };

        return openGenericType.MakeGenericType(basePropertyType);
    }

    static string GetParameterRepresentation(Type propertyType, XOfType xOfType)
        => $"Prop({GetPropertyNameRepresentation})";

    static string GetPropertyNameRepresentation(Type propertyType, XOfType xOfType)
        => xOfType == XOfType.ManyOf ? propertyType.Name.AddPluralization(makeOptional: false) : propertyType.Name;

    public string GetPropertyLineRepresentation()
    {
        var str = "public ";
        str += XOfType != XOfType.None ? $"{XOfType}<" : "";
        str += $"{PropertyTypeRepresentation}";
        str += XOfType != XOfType.None ? $">" : "";
        str += " { get; set; }";

        return str;
    }

    public string GetPropertyLineHtmlRepresentation()
    {
        var str = $"{Span("public")} ";
        str += XOfType != XOfType.None ? $"{Span(XOfType.ToString(), SpanClass.type)}{Span("<", SpanClass.identifier)}" : "";
        str += Span(PropertyTypeRepresentation, BaseIsEnum ? SpanClass.enumtype : SpanClass.type);
        str += XOfType != XOfType.None ? Span(">", SpanClass.identifier) : "";
        str += " ";
        str += Span(PropertyNameRepresentation, SpanClass.identifier);
        str += " ";
        str += $"{Span("{", SpanClass.identifier)} {Span("get")}{Span(";", SpanClass.identifier)} {Span("set")}{Span("; }", SpanClass.identifier)}";

        return str;
    }

    public override string GetParameterHtmlRepresentation() =>
        $"{Span("Prop", SpanClass.method)}{Span("(" + PropertyNameRepresentation + ")", SpanClass.identifier)}";

    public override RegexSegmentBase GetRegexSegment() =>
         new TemplatePropInfo(ResolvedType).GetCaptureGroupPropBase();

    public string GetContextMenuDisplayName()
    {
        var baseTypeDescriptor = BaseIsEnum ? "enum" : BasePropertyType.Name;

        if (ClosedGenericType?.Name is string wrapperTypeName)
            return $"{PropertyNameRepresentation}: {wrapperTypeName}<{baseTypeDescriptor}> "; 
        else
            return $"{PropertyNameRepresentation}: {baseTypeDescriptor}";
    }
}

public enum XOfType
{
    None,
    ManyOf,
    CompoundOf,
    OptionalOf,
    DynamicOf,
}