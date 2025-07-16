namespace MTGPlexer;

public class RegexTemplate
{
    public static HashSet<string> Punctuation = [".", ",", ";", "\""];
    public static HashSet<string> TerminalPunctuation = [".", ",", ";"];

    bool _noSpaces;
    Type _parentType;

    public string RenderedRegexString { get; private set; }
    public Regex Regex { get; private set; }
    public List<RegexPropInfo> RegexPropInfos { get; private set; } = [];
    public List<RegexSegmentBase> RegexSegments { get; private set; } = [];
    //public Dictionary<PropertyInfo, List<PropertyInfo>> DistilledProps { get; private set; } = [];
    public List<RegexPropBase> PropCaptureSegments => RegexSegments.OfType<RegexPropBase>().ToList();

    public RegexTemplate(Type type, params string[] templateSnippets)
    {
        if (templateSnippets is null || templateSnippets.Length == 0)
            return;

        _parentType = type;
        _noSpaces = _parentType.GetCustomAttribute<NoSpacesAttribute>() is not null;

        RegexPropInfos = GetRegexProps();

        templateSnippets
            .ToList()
            .ForEach(x => RegexSegments.Add(ResolveSnippetToPropOrTextSegment(x)));

        SetRegex();
    }

    void SetRegex()
    {
        if (_parentType.IsAssignableTo(typeof(TokenUnitOneOf)))
            RenderedRegexString = $"({string.Join('|', RegexSegments.Select(x => x.RegexString))})";
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
         _parentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(p => !p.GetMethod.IsVirtual)
        .Where(x =>
            (Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType).IsEnum
             || x.PropertyType == typeof(bool)
             || x.PropertyType == typeof(PlaceholderCapture)
             || x.PropertyType.IsAssignableTo(typeof(TokenUnit)))
        .Select(x => new RegexPropInfo(x))
        .ToList();

    //void SetDistilledProps()
    //{
    //    var placeholderCaptureProps = _parentType
    //        .GetProperties().Where(x => x.PropertyType == typeof(PlaceholderCapture))
    //        .ToList();
    //
    //    var isSinglePlaceholder = placeholderCaptureProps.Count == 1;
    //
    //    var distilledProps = _parentType
    //        .GetProperties()
    //        .Where(x => x.IsDefined(typeof(DistilledValueAttribute)));
    //
    //    foreach (var prop in distilledProps)
    //    {
    //        PropertyInfo distilledFromProp = null;
    //        var attr = prop.GetCustomAttribute<DistilledValueAttribute>();
    //
    //        if (attr.DistilledFromPropName != null)
    //            distilledFromProp = _parentType.GetProperty(attr.DistilledFromPropName);
    //        else if (isSinglePlaceholder)
    //            distilledFromProp = placeholderCaptureProps[0];
    //
    //        if (distilledFromProp is null)
    //            throw new Exception($"Distilled values must either declare a distilled-from property, or be a property of a type with exactly one PlaceholderCapture property");
    //
    //        if (!DistilledProps.TryGetValue(distilledFromProp, out var list))
    //        {
    //            list = [];
    //            DistilledProps[distilledFromProp] = list;
    //        }
    //
    //        list.Add(prop);
    //    }
    //}
}