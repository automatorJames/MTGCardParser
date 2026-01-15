namespace MTGPlexer.TokenUnitComponents;

public class RegexTemplate
{
    public static HashSet<string> Punctuation = [".", ",", ";", "\""];
    public static HashSet<char> TerminalPunctuation = ['.', ',', ';'];

    Type _containingType;

    public string RegexString { get; private set; }
    public Regex Regex { get; private set; }
    public RegexBuilder Builder { get; private set; }
    public List<RegexPropInfo> RegexPropInfos { get; private set; } = [];
    public List<RegexSegmentBase> RegexSegments { get; private set; } = [];
    public List<CaptureGroupPropBase> CaptureGroupProps => RegexSegments.OfType<CaptureGroupPropBase>().ToList();

    public RegexTemplate(Type type)
    {
        var instance = Activator.CreateInstance(type);

        if (instance is not TokenUnit tokenUnitInstance)
            throw new Exception($"Type '{type.Name}' does not derive from type '{nameof(TokenUnit)}'");

        var snippets = tokenUnitInstance.GetSnippets();

        if (snippets.Length == 0)
        {
            // If children pass no arguments or call the default parameterless base constructor,
            // we assume they want to construct snippets from their ordered properties. If no
            // properties exist, we assume they want to construct a single snippet from a pattern attribute,
            // or even the type name as a last-ditch fallback.

            var publicPropNames = type.GetPublicPropNames();

            if (publicPropNames.Length > 0)
                snippets = type.GetPublicPropNames().Select(x => (Snippet)x).ToArray();
            else if (type.GetCustomAttribute<RegexPatternAttribute>() is RegexPatternAttribute attr)
                snippets = attr.Patterns.Select(x => (Snippet)x).ToArray();
            else
                snippets = [type.Name.ToFriendlyCase(TitleDisplayOption.Lower)];

            if (snippets.Length == 0)
                throw new Exception($"Type '{type.Name}' has no snippets or valid properties");
        }

        _containingType = type;
        RegexPropInfos = GetRegexProps();

        for (int i = 0; i < snippets.Length; i++)
        {
            var snippet = snippets[i];
            var segment = ResolveSnippetToSegment(snippet);
            RegexSegments.Add(segment);
        }

        ComposeRegex();
    }

    void ComposeRegex()
    {
        ISegmentComposer composer;

        if (_containingType.IsAssignableTo(typeof(TokenUnitOneOf)))
            composer = AlternatingComposer.Instance;
        else
            composer = ConcatenatingComposer.Instance;

        RegexBuilder collector = new(_containingType);
        composer.Compose(collector, RegexSegments);

        RegexString = collector.GetMinified();
        Regex = new Regex(RegexString, RegexOptions.Compiled);
        Builder = collector;
    }

    RegexSegmentBase ResolveSnippetToSegment(Snippet templateSnippet)
    {
        var matchingProp = RegexPropInfos.FirstOrDefault(x => x.Prop.Name == templateSnippet);

        if (matchingProp != null)
            return matchingProp.GetCaptureGroupPropBase();
        else
            return new TextSegment(templateSnippet);
    }

    List<RegexPropInfo> GetRegexProps()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        return _containingType
            .GetProperties(flags)
            .Where(p => p.GetMethod is { IsVirtual: false }) // Must be non-virtual
            .Where(p => IsValidTargetType(p.PropertyType))
            .Select(p => new RegexPropInfo(p))
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
}