namespace MTGPlexer.TokenAnalysisDTOs.GraphNodes;

public record RootNode : ParentNode
{
    public Type RootType { get; set; }

    public RootNode(Type type)
    {
        RootType = type;
        PopulateChildren();
    }

    public TokenUnit(Match match)
    {
        foreach (var captureChild in CaptureChildren)
        {
            var val = captureChild.GetPropertyValue
        }
    }

    void PopulateChildren()
    {
        List<Node> children = [];
        var instance = Activator.CreateInstance(RootType);

        if (instance is not TokenUnit tokenUnitInstance)
            throw new Exception($"Type '{RootType.Name}' does not derive from type '{nameof(TokenUnit)}'");

        var snippets = tokenUnitInstance.GetSnippets();

        if (snippets.Length == 0)
        {
            // If children pass no arguments or call the default parameterless base constructor,
            // we assume they want to construct snippets from their ordered properties. If no
            // properties exist, we assume they want to construct a single snippet from a pattern attribute,
            // or even the type name as a last-ditch fallback.

            var publicPropNames = RootType.GetPublicPropNames();

            if (publicPropNames.Length > 0)
                snippets = RootType.GetPublicPropNames().Select(x => (Snippet)x).ToArray();
            else if (RootType.GetCustomAttribute<RegexPatternAttribute>() is RegexPatternAttribute attr)
                snippets = attr.Patterns.Select(x => (Snippet)x).ToArray();
            else
                snippets = [RootType.Name.ToFriendlyCase(TitleDisplayOption.Lower)];

            if (snippets.Length == 0)
                throw new Exception($"Type '{RootType.Name}' has no snippets or valid properties");
        }

        Children = snippets.Select(SnippetToNode).ToList();

        for (int i = 0; i < snippets.Length; i++)
        {
            var snippet = snippets[i];
            var matchingProp = targetProps.FirstOrDefault(x => x.Name == snippet.Text);

            if (matchingProp != null)
                return matchingProp.GetCaptureGroupPropBase(templateSnippet.Proptions);
            else
                return new TextSegment(templateSnippet);

            var segment = ResolveSnippetToSegment(snippet);

            RegexSegments.Add(segment);
        }

        ComposeRegex();
    }

    static Node SnippetToNode(Snippet snippet)
    {
        if (snippet is PropertySnippet propertySnippet)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertySnippet.Prop.PropertyType) ?? propertySnippet.Prop.PropertyType;

            return underlyingType switch
            {
                { IsEnum: true } => new EnumNode(propertySnippet)
                { } t when t.IsAssignableTo(typeof(ManyOf)) => TemplatePropType.ManyOf,
                { } t when t.IsAssignableTo(typeof(CompoundOf)) => TemplatePropType.CompoundOf,
                { } t when t.IsAssignableTo(typeof(OneOf)) => TemplatePropType.OneOf,
                { } t when t.IsAssignableTo(typeof(OptionalOf)) => TemplatePropType.OptionalOf,
                { } t when t == typeof(PlaceholderCapture) => TemplatePropType.Placeholder,
                { } t when t.IsAssignableTo(typeof(DynamicOf)) => TemplatePropType.Dynamic,
                { } t when t == typeof(bool) => TemplatePropType.Bool,
                { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => TemplatePropType.TokenUnitOneOf,
                { } t when typeof(TokenUnit).IsAssignableFrom(t) => TemplatePropType.TokenUnit,
                _ => throw new Exception($"{type.Name} is not a valid {nameof(TemplatePropType)} type")
            };
        }
        else
            return new TextNode(snippet);
    }

    void ComposeRegex()
    {
        var builderWithComposition = CompositionFactory.Compose(RegexSegments, _containingType);
        RegexString = builderWithComposition.GetMinified();
        Regex = new Regex(RegexString, RegexOptions.Compiled);
        Builder = builderWithComposition;
    }

    List<PropertyInfo> GetTargetProps()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        return RootType
            .GetProperties(flags)
            .Where(p => p.GetMethod is { IsVirtual: false }) // Must be non-virtual
            .Where(p => IsValidTargetType(p.PropertyType))
            .ToList();
    }

    static bool IsValidTargetType(Type type)
    {
        // Unwrap Nullable
        type = Nullable.GetUnderlyingType(type) ?? type;

        // Handle Generic Wrappers
        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();

            // Single-argument wrappers: ManyOf<T> or CompoundOf<T>
            if (genericDef == typeof(ManyOf<>) || genericDef == typeof(CompoundOf<>) || genericDef == typeof(OptionalOf<>))
                return IsValidTargetType(type.GetGenericArguments()[0]);

            // Multi-argument wrappers: OneOf<T1, T2> or OneOf<T1, T2, T3>
            if (genericDef == typeof(OneOf<,>) || genericDef == typeof(OneOf<,,>))
                return type.GetGenericArguments().All(IsValidTargetType);

        }

        // Check Leaf/Base Types
        return IsLeafTargetType(type);
    }

    static bool IsLeafTargetType(Type type)
    {
        return type.IsEnum
            || type == typeof(bool)
            || type == typeof(PlaceholderCapture)
            || type.IsAssignableTo(typeof(DynamicOf))
            || typeof(TokenUnit).IsAssignableFrom(type);
    }

    public override void ComposeRegexLines(RegexBuilder collector)
    {
        throw new NotImplementedException();
    }
}