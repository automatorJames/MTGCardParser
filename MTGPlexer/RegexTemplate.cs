namespace MTGPlexer;

public class RegexTemplate
{
    public static HashSet<string> Punctuation = [".", ",", ";", "\""];
    public static HashSet<string> TerminalPunctuation = [".", ",", ";"];

    bool _noSpaces;
    Type _containingType;

    public string RenderedRegexString { get; private set; }
    public Regex Regex { get; private set; }
    public List<RegexPropInfo> RegexPropInfos { get; private set; } = [];
    public List<RegexSegmentBase> RegexSegments { get; private set; } = [];
    public List<RegexPropBase> PropCaptureSegments => RegexSegments.OfType<RegexPropBase>().ToList();

    public RegexTemplate(Type type, params string[] templateSnippets)
    {
        if (templateSnippets is null || templateSnippets.Length == 0)
            return;

        _containingType = type;
        _noSpaces = _containingType.GetCustomAttribute<NoSpacesAttribute>() is not null;

        RegexPropInfos = GetRegexProps();

        templateSnippets
            .ToList()
            .ForEach(x => RegexSegments.Add(ResolveSnippetToPropOrTextSegment(x)));

        SetRegex();
    }

    public RegexTemplate(Type tokenUnitOneOfType)
    {
        if (!tokenUnitOneOfType.IsAssignableTo(typeof(TokenUnitOneOf)))
            throw new Exception($"{tokenUnitOneOfType.Name} isn't derived from {nameof(TokenUnitOneOf)}");

        _containingType = tokenUnitOneOfType;
        RegexPropInfos = GetRegexProps();
        List<string> captureSections = [];

        var tokenUnitChildProps = tokenUnitOneOfType
            .GetProps()
            .Where(x => x.PropertyType.IsAssignableTo(typeof(TokenUnit)));

        tokenUnitChildProps
            .Select(x => x.Name)
            .ToList()
            .ForEach(x => RegexSegments.Add(ResolveSnippetToPropOrTextSegment(x)));

        var tokenUnitChildPropTypes = tokenUnitChildProps
            .Select(x => Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType);

        foreach (var childType in tokenUnitChildPropTypes)
        {
            var template = TokenTypeRegistry.GetTypeTemplate(childType);
            captureSections.Add(template.RenderedRegexString);
        }

        var headerComment = TokenUnitOneOf.GetTokenUnitOneOfRegexHeaderComment(tokenUnitOneOfType);
        RenderedRegexString = $"({headerComment}{string.Join('|', captureSections)})";
        Regex = new Regex(RenderedRegexString);
    }

    void SetRegex()
    {
        if (_containingType.IsAssignableTo(typeof(TokenUnitOneOf)))
        {
            var template = TokenTypeRegistry.GetTypeTemplate(_containingType);
            RenderedRegexString = template.RenderedRegexString;
            Regex = template.Regex;
        }
            
        else
        {
            for (int i = 0; i < RegexSegments.Count; i++)
            {
                var segment = RegexSegments[i];
                RenderedRegexString += segment.RegexString;

                var shouldAddSpace =
                    !_noSpaces
                    && i < RegexSegments.Count - 1
                    && !(segment is BoolRegexProp)
                    && !TerminalPunctuation.Contains(segment.RegexString);

                if (shouldAddSpace)
                    RenderedRegexString += " ";
            }
        }

        if (_containingType.GetCustomAttribute<NoWordBoundaryAttribute>() == null)
            RenderedRegexString = $@"\b{RenderedRegexString}\b";

        Regex = new Regex(RenderedRegexString, RegexOptions.Compiled);
    }

    RegexSegmentBase ResolveSnippetToPropOrTextSegment(string templateSnippet)
    {
        var matchingProp = RegexPropInfos.FirstOrDefault(x => x.Name == templateSnippet);

        if (matchingProp is not null)
        {
            return matchingProp.RegexPropType switch
            {
                RegexPropType.TokenUnit => new TokenRegexProp(matchingProp),
                RegexPropType.TokenUnitOneOf => new TokenRegexOneOfProp(matchingProp),
                RegexPropType.Enum => new EnumRegexProp(matchingProp),
                RegexPropType.Bool => new BoolRegexProp(matchingProp),
                RegexPropType.Placeholder => new PlaceholderRegexProp(matchingProp),
                _ => throw new Exception($"Prop type '{matchingProp.Prop.PropertyType.Name}' is not a valid RegexProp type")
            };
        }
        else
            return new TextSegment(templateSnippet);
    }

    List<RegexPropInfo> GetRegexProps() =>
         _containingType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(p => !p.GetMethod.IsVirtual)
        .Where(x =>
            (Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType).IsEnum
             || x.PropertyType == typeof(bool)
             || x.PropertyType == typeof(PlaceholderCapture)
             || x.PropertyType.IsAssignableTo(typeof(TokenUnit)))
        .Select(x => new RegexPropInfo(x))
        .ToList();
}