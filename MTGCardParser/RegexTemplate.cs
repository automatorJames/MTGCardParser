namespace MTGCardParser;

public class RegexTemplate
{
    protected bool _noSpaces;

    public Dictionary<PropertyInfo, IRegexSegment> PropRegexSegments { get; set; } = new();
    public List<IRegexSegment> RegexSegments { get; set; } = new();
    public string RenderedRegex { get; set; }

    public RegexTemplate()
    {
    }

    public RegexTemplate(RegexTemplate source)
    {
        PropRegexSegments = source.PropRegexSegments.ToDictionary();
        RegexSegments = source.RegexSegments.ToList();
        RenderedRegex = source.RenderedRegex;
    }
}

public class RegexTemplate<T> : RegexTemplate
{
    public RegexTemplate(params string[] templateSnippets)
    {
        var type = typeof(T);
        var props = type.GetPropertiesForCapture();
        _noSpaces = type.GetCustomAttribute<NoSpacesAttribute>() is not null;

        for (int i = 0; i < templateSnippets.Length; i++)
        {
            string snippet = templateSnippets[i];
            var matchingProp = props.FirstOrDefault(x => x.Name == snippet);

            if (matchingProp != null)
            {
                IRegexSegment regexSegment;

                var underlyingType = Nullable.GetUnderlyingType(matchingProp.PropertyType) ?? matchingProp.PropertyType;

                if (underlyingType.IsEnum)
                    regexSegment = new EnumCaptureGroup(underlyingType);
                else if (underlyingType.IsAssignableTo(typeof(ITokenCapture)))
                    regexSegment = TokenCaptureSubPropertyToCaptureGroup(matchingProp);
                else
                    regexSegment = PropertyToCaptureGroup(matchingProp);

                RegexSegments.Add(regexSegment);
                PropRegexSegments[matchingProp] = regexSegment;
            }
            else
                RegexSegments.Add(new RegexSegment(snippet));
        }

        for (int i = 0; i < RegexSegments.Count; i++)
        {
            var segment = RegexSegments[i];
            var segmentString = segment.RegexString;
            RenderedRegex += segmentString;

            var shouldAddSpace =
                !_noSpaces
                && i < RegexSegments.Count - 1
                && !(segment is CaptureGroup capGroup && capGroup.CapturePropType == CapturePropType.Bool)
                && !TerminalPunctuation.Contains(segmentString);

            if (shouldAddSpace)
                RenderedRegex += " ";
        }

        // We don't need word boundaries where there are spaces (this step just improves regex human readability)
        RenderedRegex = RenderedRegex.Replace(@"\b \b", " ");
    }

    static HashSet<string> TerminalPunctuation = [".", ",", ";"];

    CaptureGroup TokenCaptureSubPropertyToCaptureGroup(PropertyInfo subTokenCaptureProp)
    {
        var instanceOfPropType = (ITokenCapture)Activator.CreateInstance(subTokenCaptureProp.PropertyType);
        return new CaptureGroup(subTokenCaptureProp.Name, [instanceOfPropType.RegexTemplate.RenderedRegex], CapturePropType.TokenCapture, new());
    }

    CaptureGroup PropertyToCaptureGroup(PropertyInfo prop)
    {
        var captureGroupType = 
            prop.PropertyType == typeof(TokenSegment) ? CapturePropType.TokenSegment
            : prop.PropertyType == typeof(bool) ? CapturePropType.Bool
            : throw new Exception($"Property type {prop.PropertyType.Name} not supported");

        var regexPatternAttribute = prop.GetCustomAttribute<RegexPatternAttribute>();
        var patterns = regexPatternAttribute?.Patterns ?? [prop.Name];
        var groupIsOptional = prop.PropertyType == typeof(bool);

        return new CaptureGroup(prop.Name, regexPatternAttribute.Patterns, captureGroupType, new());

    }
}

