using System.Text;

namespace MTGCardParser;

public class RegexTemplate
{
    protected bool _noSpaces;

    public Dictionary<PropertyInfo, IRegexSegment> PropRegexSegments { get; set; } = new();
    public List<IRegexSegment> RegexSegments { get; set; } = new();
    public string RenderedRegex { get; set; }
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

        //var renderedRegex = JoinWithSmartSpacing(captureGroupTemplates);
        //renderedRegex = WrapInSmartWordBoundaries(renderedRegex);

        //if (type != typeof(Punctuation))
        //    renderedRegex = @"\b" + renderedRegex + @"\b";


        //for (int i = 0; i < RegexSegments.Count; i++)
        //{
        //    var segment = RegexSegments[i];
        //
        //    if (segment is RegexSegment simpleSegment)
        //        RenderedRegex += simpleSegment.PositionalRegexPattern[RenderingPosition.Any];
        //    else
        //    {
        //        var position =
        //            i == 0 ? RenderingPosition.First
        //            : i == RegexSegments.Count - 1 ? RenderingPosition.Last
        //            : RenderingPosition.Middle;
        //
        //        RenderedRegex += segment.PositionalRegexPattern[position];
        //    }
        //}


        for (int i = 0; i < RegexSegments.Count; i++)
        {
            var segment = RegexSegments[i];
            var segmentString = segment.RegexString;

            //if (removeNextWordBoundary && RenderedRegex.StartsWith(@"\b"))
            //    segmentString = segmentString.Substring(2, segmentString.Length - 2);

            RenderedRegex += segmentString;

            if (i < RegexSegments.Count - 1 && !_noSpaces && !(segment is CaptureGroup capGroup && capGroup.CapturePropType == CapturePropType.Bool))
                RenderedRegex += " ";


            //if (i < RegexSegments.Count - 1 && segment is not RegexSegment && RegexSegments[i + 1] is not RegexSegment)
            //    RenderedRegex += " ";
        }

        // Don't need word boundaries where there are spaces (this step just improves regex human readability)
        RenderedRegex = RenderedRegex.Replace(@"\b \b", " ");

    }

    //EnumCaptureGroup EnumToCaptureGroup(Type underlyingEnumType)
    //{
    //    List<string> alternatives = new();
    //    EnumRegexes[underlyingEnumType] = new();
    //    var enumRegOptions = underlyingEnumType.GetCustomAttribute<RegexOptionsAttribute>() ?? new();
    //    var enumValues = Enum.GetValues(underlyingEnumType).Cast<object>();
    //
    //    foreach (var enumValue in enumValues)
    //    {
    //        var enumAsString = enumValue.ToString();
    //        var regexPatternAttribute = underlyingEnumType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();
    //
    //        // Check if enum member has a specified string[] of patterns
    //        if (regexPatternAttribute != null)
    //            alternatives.AddRange(regexPatternAttribute.Patterns);
    //        // Otherwise add the stringified enum iteself
    //        else
    //            alternatives.Add(enumAsString.ToLower());
    //    }
    //
    //    return new CaptureGroup(underlyingEnumType.Name, alternatives, CapturePropType.Enum, enumRegOptions);
    //}

    CaptureGroup TokenCaptureSubPropertyToCaptureGroup(PropertyInfo subTokenCaptureProp)
    {
        var instanceOfPropType = (ITokenCapture)Activator.CreateInstance(subTokenCaptureProp.PropertyType);
        return new CaptureGroup(subTokenCaptureProp.Name, [instanceOfPropType.RenderedRegex], CapturePropType.TokenCapture, new());
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

    string GetCaptureGroupOfAlternatives(IEnumerable<string> items, string namedCaptureGroup, bool wrapInWordBoundaries, bool groupIsOptional, bool prependSpace = false, bool appendSpace = false)
    {
        var str = $@"(?<{namedCaptureGroup}>{(prependSpace ? " " : "")}{string.Join("|", items.OrderByDescending(s => s.Length))}{(appendSpace ? " " : "")})";

        if (groupIsOptional)
            str += "?";

        return str;
    }

    public static string Pluralize(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            throw new ArgumentNullException(nameof(word));

        if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) && word.Length > 1 && !"aeiou".Contains(char.ToLower(word[word.Length - 2])))
            word = word.Substring(0, word.Length - 1) + "(ies)";
        else if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            word += "(es)";
        else word += "(s)";

        return word + "?";
    }

    static readonly HashSet<string> TerminalPunctuation = new()
    {
        ".", ",", ";"
    };

    static readonly HashSet<string> MatchedPairChars = new()
    {
        "\""
    };


    string JoinWithSmartSpacing(List<string> segments)
    {
        var str = "";
        int matchedPairCharCount = 0;

        for (int i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];

            if (i > 0)
            {
                bool spaceBefore = !TerminalPunctuation.Contains(segment) && matchedPairCharCount % 2 == 0;
                str += spaceBefore ? " " : "";
            }

            matchedPairCharCount += MatchedPairChars.Contains(segment) ? 1 : 0;
            str += segment;
        }

        if (matchedPairCharCount % 2 != 0)
            throw new Exception("Paired chars not matched");

        return str.Trim();
    }
}

