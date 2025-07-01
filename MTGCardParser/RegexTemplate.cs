using MTGCardParser.RegexSegmentDTOs.Interfaces;

namespace MTGCardParser;

public class RegexTemplate
{
    protected bool _noSpaces;
    public List<CaptureProp> CaptureProps { get; set; }
    public string RenderedRegex { get; set; }
    public List<IRegexSegment> RegexSegments { get; set; } = new();
    public List<IPropRegexSegment> PropCaptureSegments => RegexSegments.OfType<IPropRegexSegment>().ToList();
    public List<TokenCaptureAlternativeSet> AlternativePropCaptureSets => RegexSegments.OfType<TokenCaptureAlternativeSet>().ToList();

    public RegexTemplate()
    {
    }

    public RegexTemplate(RegexTemplate source)
    {
        RegexSegments = source.RegexSegments.ToList();
        RenderedRegex = source.RenderedRegex;
    }
}

public class RegexTemplate<T> : RegexTemplate
{
    public RegexTemplate(params object[] templateSnippets)
    {
        CaptureProps = GetPropertiesForCapture();
        _noSpaces = typeof(T).GetCustomAttribute<NoSpacesAttribute>() is not null;

        foreach (var snippetObj in templateSnippets)
        {
            IRegexSegment resolvedSegment;

            if (snippetObj is string snippetString)
                resolvedSegment = ResolveSnippetToRegexSegment(snippetString);

            else if (snippetObj is CaptureAlternatives captureAlternatives)
            {
                List<TokenCaptureSegment> alternativeTokenCaptureSegments = new();

                foreach (var alternative in captureAlternatives.Names)
                {
                    var resolvedAlternative = (TokenCaptureSegment)ResolveSnippetToRegexSegment(alternative, forceResolveTokenUnit: true);
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
            RenderedRegex += segment.RegexString;

            var shouldAddSpace =
                !_noSpaces
                && i < RegexSegments.Count - 1
                && !(segment is ScalarCaptureSegment propCap && propCap.IsBool)
                && !TerminalPunctuation.Contains(segment.RegexString);

            if (shouldAddSpace)
                RenderedRegex += " ";
        }

        // We don't need word boundaries where there are spaces (this step just improves regex human readability)
        RenderedRegex = RenderedRegex.Replace(@"\b \b", " ");
    }

    static HashSet<string> TerminalPunctuation = [".", ",", ";"];

    CaptureProp GetMatchingProp(string propName, bool isRequiredToExistOnType = false)
    {
        var matchingProp = CaptureProps.FirstOrDefault(x => x.Name == propName);

        if (isRequiredToExistOnType && matchingProp is null)
            throw new Exception($"Property {propName} is required, but not found on type '{typeof(T).Name}'");

        return matchingProp;
    }

    IRegexSegment ResolveSnippetToRegexSegment(string templateSnippet, bool forceResolveTokenUnit = false)
    {
        var matchingProp = GetMatchingProp(templateSnippet, isRequiredToExistOnType: forceResolveTokenUnit);

        if (matchingProp is not null)
        {
            var isTokenUnitType = matchingProp.UnderlyingType.IsAssignableTo(typeof(ITokenUnit));

            if (forceResolveTokenUnit && matchingProp.CapturePropType != CapturePropType.TokenUnit)
                throw new Exception($"Prop type {matchingProp.UnderlyingType.Name} is required to implement ({nameof(ITokenUnit)})");

            return matchingProp.CapturePropType switch
            {
                CapturePropType.TokenUnit => new TokenCaptureSegment(matchingProp),
                CapturePropType.Enum => new EnumCaptureSegment(matchingProp),
                _ => new ScalarCaptureSegment(matchingProp),
            };
        }
        else
            return new TextSegment(templateSnippet);
    }

    List<CaptureProp> GetPropertiesForCapture() =>
         typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(p => !p.GetMethod.IsVirtual)
        .Where(x => 
            (Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType).IsEnum 
             || x.PropertyType == typeof(bool) 
             || x.PropertyType == typeof(CapturedTextSegment) 
             || x.PropertyType.IsAssignableTo(typeof(ITokenUnit)))
        .Select(x => new CaptureProp(x))
        .ToList();
}

