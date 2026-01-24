
namespace MTGPlexer.CommonDTOs;

public record EditorPropertySnippet : EditorBlockSnippet
{
    public Type BasePropertyType { get; init; }
    public XOfType XOfType { get; init; }
    public bool BaseIsEnum { get; init; }
    public Type ClosedGenericType { get; init; }
    public string PropertyTypeRepresentation { get; init; }
    public string PropertyNameRepresentation { get; init; }
    public Type ResolvedType => ClosedGenericType ?? BasePropertyType;

    public EditorPropertySnippet(Type basePropertyType, XOfType xOfType, string id)
        : base(
            editorRepresentation: $"@{(xOfType != XOfType.None ? $"{xOfType}<{basePropertyType.Name}>" : basePropertyType.Name)}",
            parameterRepresentation: $"Prop({(xOfType == XOfType.ManyOf ? basePropertyType.Name.AddPluralization(false) : basePropertyType.Name)})",
            id: id)
    {
        BasePropertyType = basePropertyType;
        XOfType = xOfType;
        BaseIsEnum = basePropertyType.IsEnum;

        PropertyNameRepresentation = XOfType == XOfType.ManyOf
            ? basePropertyType.Name.AddPluralization(false)
            : basePropertyType.Name;

        PropertyTypeRepresentation = basePropertyType.Name;

        ClosedGenericType = XOfType switch
        {
            XOfType.None => null,
            XOfType.ManyOf => typeof(ManyOf<>).MakeGenericType(basePropertyType),
            XOfType.CompoundOf => typeof(CompoundOf<>).MakeGenericType(basePropertyType),
            XOfType.OptionalOf => typeof(OptionalOf<>).MakeGenericType(basePropertyType),
            XOfType.DynamicOf => throw new NotImplementedException("DynamicOf not supported"),
            _ => throw new ArgumentException($"Unsupported type: {xOfType}")
        };
    }

    public string GetPropertyLineRepresentation()
    {
        var wrapper = XOfType != XOfType.None ? $"{XOfType}<" : "";
        var closer = XOfType != XOfType.None ? ">" : "";
        return $"public {wrapper}{PropertyTypeRepresentation}{closer} {PropertyNameRepresentation} {{ get; set; }}";
    }

    public string GetPropertyLineHtmlRepresentation()
    {
        var typeStyle = BaseIsEnum ? SpanClass.enumtype : SpanClass.type;
        var part1 = $"{Span("public")} ";

        if (XOfType != XOfType.None)
            part1 += $"{Span(XOfType.ToString(), SpanClass.type)}{Span("<", SpanClass.identifier)}";

        part1 += Span(PropertyTypeRepresentation, typeStyle);

        if (XOfType != XOfType.None)
            part1 += Span(">", SpanClass.identifier);

        return $"{part1} {Span(PropertyNameRepresentation, SpanClass.identifier)} " +
               $"{Span("{", SpanClass.identifier)} {Span("get")}{Span(";", SpanClass.identifier)} " +
               $"{Span("set")}{Span("; }", SpanClass.identifier)}";
    }

    public override string GetParameterHtmlRepresentation() =>
        $"{Span("Prop", SpanClass.method)}{Span($"({PropertyNameRepresentation})", SpanClass.identifier)}";

    public override RegexSegmentBase GetRegexSegment() =>
         new TemplatePropInfo(ResolvedType).GetCaptureGroupPropBase();

    public string GetContextMenuDisplayName()
    {
        var typeDesc = BaseIsEnum ? "enum" : BasePropertyType.Name;
        return ClosedGenericType != null
            ? $"{PropertyNameRepresentation}: {ClosedGenericType.Name}<{typeDesc}>"
            : $"{PropertyNameRepresentation}: {typeDesc}";
    }

    public EditorPropertySnippet ConvertToXOfType(XOfType xOfType)
    {
        if (xOfType == XOfType)
            return this;

        return new(BasePropertyType, xOfType, Id);
    }
}