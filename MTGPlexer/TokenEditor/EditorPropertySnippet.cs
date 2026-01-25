namespace MTGPlexer.TokenEditor;

public record EditorPropertySnippet : EditorBlockSnippet
{
    public Type BasePropertyType { get; init; }
    public XOfType XOfType { get; init; }
    public Proptions Proptions { get; init; }
    public bool BaseIsEnum { get; init; }
    public Type ClosedGenericType { get; init; }
    public string PropertyTypeRepresentation { get; init; }
    public string PropertyNameRepresentation { get; init; }
    public Type ResolvedType => ClosedGenericType ?? BasePropertyType;

    public EditorPropertySnippet(Type basePropertyType, XOfType xOfType, string id, Proptions proptions = Proptions.None)
        : base(
            editorRepresentation: GetEditorRepresentation(basePropertyType, xOfType, proptions),
            parameterRepresentation: GetParameterRepresentation(basePropertyType, xOfType, proptions),
            id: id)
    {
        BasePropertyType = basePropertyType;
        XOfType = xOfType;
        Proptions = proptions;
        BaseIsEnum = basePropertyType.IsEnum;
        PropertyNameRepresentation = GetPropertyNameRepresentation(basePropertyType, xOfType);
        PropertyTypeRepresentation = basePropertyType.Name;

        ClosedGenericType = XOfType switch
        {
            XOfType.None => null,
            XOfType.ManyOf => typeof(ManyOf<>).MakeGenericType(basePropertyType),
            XOfType.CompoundOf => typeof(CompoundOf<>).MakeGenericType(basePropertyType),
            XOfType.OptionalOf => typeof(OptionalOf<>).MakeGenericType(basePropertyType),
            XOfType.DynamicOf => throw new NotImplementedException("DynamicOf not yet supported"),
            _ => throw new ArgumentException($"Unsupported type: {xOfType}")
        };
    }

    static string GetEditorRepresentation(Type basePropertyType, XOfType xOfType, Proptions proptions)
    {
        var representation = basePropertyType.Name;

        // Wrap in XOfType if any
        if (xOfType != XOfType.None)
            representation = $"{xOfType}<{representation}>";

        if (proptions != Proptions.None)
        {
            var proptionsPresent = proptions.GetSetFlags();

            // Specify proptions in "()" method-like enclosure
            if (proptionsPresent.Any())
                representation = $"{representation}({string.Join(", ", proptionsPresent)})";
        }

        return representation;
    }

    static string GetParameterRepresentation(Type basePropertyType, XOfType xOfType, Proptions proptions)
    {
        var representation = GetPropertyNameRepresentation(basePropertyType, xOfType);

        if (proptions != Proptions.None)
        {
            var proptionsPresent = proptions.GetSetFlags();
            var proptionsAsParameters = proptionsPresent.Select(x => $"Proptions.{x}");
            representation += ", " + string.Join(" | ", proptionsAsParameters);
        }

        return $"Prop({representation})";
    }

    static string GetPropertyNameRepresentation(Type basePropertyType, XOfType xOfType) =>
        xOfType == XOfType.ManyOf
            ? basePropertyType.Name.AddPluralization(false)
            : basePropertyType.Name;

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

    public override string GetParameterHtmlRepresentation()
    {
        var representation = $"{Span("Prop", SpanClass.method)}{Span($"({PropertyNameRepresentation}", SpanClass.identifier)}";

        if (Proptions != Proptions.None)
        {
            representation += Span(", ", SpanClass.identifier);

            var flags = Proptions.GetSetFlags();

            for (int i = 0; i < flags.Length; i++)
            {
                var flag = flags[i];
                representation += $"{Span(nameof(Proptions), SpanClass.enumtype)}{Span($".{flag}", SpanClass.identifier)}";

                if (i < flags.Length - 1)
                    representation += Span(" | ", SpanClass.identifier);
            }
        }

        representation += Span(")", SpanClass.identifier);

        return representation;
    }

    public override RegexSegmentBase GetRegexSegment() =>
         new TemplatePropInfo(ResolvedType, PropertyNameRepresentation).GetCaptureGroupPropBase(Proptions);

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

    public EditorPropertySnippet UpdateProptions(Proptions oneHotToggleProptions)
    {
        if ((oneHotToggleProptions & (oneHotToggleProptions - 1)) != 0)
            throw new ArgumentException("Expected a one-hot flag value.", nameof(oneHotToggleProptions));

        // Invert (toggle) the specified flag, leaving other flags as-is
        var updatedProptions = Proptions ^ oneHotToggleProptions;

        return new(BasePropertyType, XOfType, Id, updatedProptions);
    }

    public enum ContextActionType
    {
        Delete,
        ConvertToOneOf,
        ConvertToManyOf,
        ConvertToCompoundOf
    }
}