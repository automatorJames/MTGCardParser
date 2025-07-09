namespace MTGCardParser;

public class RegexTemplate
{
    protected bool _noSpaces;
    protected List<RegexPropInfo> _regexPropInfos;

    public string RenderedRegexString { get; set; }
    public List<RegexSegmentBase> RegexSegments { get; set; } = new();
    public List<RegexPropBase> PropCaptureSegments => RegexSegments.OfType<RegexPropBase>().ToList();
    public List<TokenCaptureAlternativeSet> AlternativePropCaptureSets => RegexSegments.OfType<TokenCaptureAlternativeSet>().ToList();
    public List<TokenRegexProp> AllUnwrappedTokenCaptureSegments =>
        RegexSegments.OfType<TokenRegexProp>()
        .Concat(AlternativePropCaptureSets.SelectMany(x => x.Alternatives))
        .ToList();

    public RegexTemplate()
    {
    }

    public RegexTemplate(RegexTemplate source)
    {
        RegexSegments = source.RegexSegments.ToList();
        RenderedRegexString = source.RenderedRegexString;
    }

    public IEnumerable<RegexPropInfo> GetOrderedProps()
    {
        foreach (var segment in RegexSegments)
            if (segment is RegexPropBase propSegment)
                yield return propSegment.RegexPropInfo;
            else if (segment is TokenCaptureAlternativeSet alternativeSet)
                foreach (var altPropSegment in alternativeSet.Alternatives)
                    yield return altPropSegment.RegexPropInfo;
    }
}

public class RegexTemplate<T> : RegexTemplate
{
    public RegexTemplate(params object[] templateSnippets)
    {
        _regexPropInfos = GetRegexProps();
        _noSpaces = typeof(T).GetCustomAttribute<NoSpacesAttribute>() is not null;

        foreach (var snippetObj in templateSnippets)
        {
            RegexSegmentBase resolvedSegment;

            if (snippetObj is string snippetString)
                resolvedSegment = ResolveSnippetToRegexProp(snippetString);

            else if (snippetObj is AlternativeTokenUnits captureAlternatives)
            {
                List<TokenRegexProp> alternativeTokenCaptureSegments = new();

                foreach (var alternative in captureAlternatives.Names)
                {
                    var resolvedAlternative = (TokenRegexProp)ResolveSnippetToRegexProp(alternative, forceResolveTokenUnit: true);
                    alternativeTokenCaptureSegments.Add(resolvedAlternative);
                }

                resolvedSegment = new TokenCaptureAlternativeSet(alternativeTokenCaptureSegments);
            }
            else
                throw new Exception($"Each snippet must be of type string or CaptureAlternatives");

            RegexSegments.Add(resolvedSegment);
        }

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

        // We don't need word boundaries where there are spaces (this step just improves regex human readability)
        RenderedRegexString = RenderedRegexString.Replace(@"\b \b", " ");
    }

    static HashSet<string> TerminalPunctuation = [".", ",", ";"];

    RegexPropInfo GetMatchingProp(string propName, bool isRequiredToExistOnType = false)
    {
        var matchingProp = _regexPropInfos.FirstOrDefault(x => x.Name == propName);

        if (isRequiredToExistOnType && matchingProp is null)
            throw new Exception($"Property {propName} is required, but not found on type '{typeof(T).Name}'");

        return matchingProp;
    }

    RegexSegmentBase ResolveSnippetToRegexProp(string templateSnippet, bool forceResolveTokenUnit = false)
    {
        var matchingProp = GetMatchingProp(templateSnippet, isRequiredToExistOnType: forceResolveTokenUnit);

        if (matchingProp is not null)
        {
            var isTokenUnitType = matchingProp.UnderlyingType.IsAssignableTo(typeof(TokenUnit));

            if (forceResolveTokenUnit && matchingProp.RegexPropType != RegexPropType.TokenUnit)
                throw new Exception($"Prop type {matchingProp.UnderlyingType.Name} is required to implement ({nameof(TokenUnit)})");

            return matchingProp.RegexPropType switch
            {
                RegexPropType.TokenUnit => new TokenRegexProp(matchingProp),
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
         typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(p => !p.GetMethod.IsVirtual)
        .Where(x =>
            (Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType).IsEnum
             || x.PropertyType == typeof(bool)
             || x.PropertyType == typeof(PlaceholderCapture)
             || x.PropertyType.IsAssignableTo(typeof(TokenUnit)))
        .Select(x => new RegexPropInfo(x))
        .ToList();
}

