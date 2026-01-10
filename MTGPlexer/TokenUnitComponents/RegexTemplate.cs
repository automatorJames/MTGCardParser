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
            // we assume they want to construct snippets from their ordered properties.

            snippets = type.GetPublicPropNames().Select(x => (Snippet)x).ToArray();

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
        static bool IsTarget(Type t)
        {
            var underlyingType = Nullable.GetUnderlyingType(t) ?? t;

            if (underlyingType.IsGenericType && (underlyingType.GetGenericTypeDefinition() == typeof(ManyOf<>) || underlyingType.GetGenericTypeDefinition() == typeof(CompoundOf<>)))
                underlyingType = underlyingType.GetGenericArguments()[0];

            return underlyingType.IsEnum
                || underlyingType == typeof(bool)
                || underlyingType == typeof(PlaceholderCapture)
                || underlyingType.IsAssignableTo(typeof(DynamicCapture))
                || typeof(TokenUnit).IsAssignableFrom(underlyingType);
        }

        return _containingType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(p => p.GetMethod is { IsVirtual: false })
            .Where(p =>
                IsTarget(p.PropertyType)
                || (p.PropertyType.IsArray && IsTarget(p.PropertyType.GetElementType()!)))
            .Select(p => new RegexPropInfo(p))
            .ToList();
    }
}