namespace MTGPlexer;

public class RegexTemplate
{
    public static HashSet<string> Punctuation = [".", ",", ";", "\""];
    public static HashSet<string> TerminalPunctuation = [".", ",", ";"];

    bool _noSpaces;
    Type _containingType;

    public string RegexString { get; private set; }
    public string RegexStringNoWordBoundaries { get; private set; }
    public string RegexStringNoCaptureGroups { get; private set; }
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

    public RegexTemplate(Type type)
    {
        if (!type.IsAssignableTo(typeof(TokenUnitOneOf)) && !type.IsAssignableTo(typeof(ManyToken)))
            throw new Exception($"{type.Name} must derive from {nameof(TokenUnitOneOf)} or ManyToken");

        _containingType = type;
        RegexPropInfos = GetRegexProps();
        List<string> captureSections = [];

        var tokenUnitChildProps = type
            .GetProps()
            .Where(x => x.PropertyType.IsAssignableTo(typeof(TokenUnit)));

        tokenUnitChildProps
            .Select(x => x.Name)
            .ToList()
            .ForEach(x => RegexSegments.Add(ResolveSnippetToPropOrTextSegment(x)));

        var tokenUnitChildPropTypes = tokenUnitChildProps
            .Select(x => Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType);

        if (type.IsAssignableTo(typeof(TokenUnitOneOf)))
        {
            foreach (var childType in tokenUnitChildPropTypes)
            {
                var template = TokenTypeRegistry.GetTypeTemplate(childType);

                var groupRegexToAdd = childType.IsDefined(typeof(NoWordBoundaryAttribute)) ? 
                    template.RegexStringNoWordBoundaries 
                    : $@"\b{template.RegexStringNoWordBoundaries}\b";

                captureSections.Add(groupRegexToAdd);
            }

            var headerComment = TokenUnitOneOf.GetTokenUnitOneOfRegexHeaderComment(type);
            RegexString = $"({headerComment}{string.Join('|', captureSections)})";
        }
        else
        {
            var genericType = type.GenericTypeArguments[0];
            var singleRegex = TokenTypeRegistry.GetTypeTemplate(genericType).RegexStringNoCaptureGroups;
            RegexString = $"(?<{genericType.Name}_Item>{singleRegex})(?:,? (?<{genericType.Name}_Item>{singleRegex}))*(?:,? (?<{nameof(Conjunction)}>and|or)) (?<{genericType.Name}_Item>{singleRegex})";

            var altRegexString = $"(?<{genericType.Name}_Item>{singleRegex})(?:(?:,? (?<{genericType.Name}_Item>{singleRegex}))*(?:,? (?<{nameof(Conjunction)}>and|or)) (?<{genericType.Name}_Item>{singleRegex}))?";
        }

        RegexStringNoWordBoundaries = RegexString;
        RegexStringNoCaptureGroups = StripNamedCaptureGroups(RegexString);
        Regex = new Regex(RegexString);
    }


    void SetRegex()
    {
        if (_containingType.IsAssignableTo(typeof(TokenUnitOneOf)))
        {
            var template = TokenTypeRegistry.GetTypeTemplate(_containingType);
            RegexString = template.RegexString;
            Regex = template.Regex;
        }
            
        else
        {
            for (int i = 0; i < RegexSegments.Count; i++)
            {
                var segment = RegexSegments[i];
                RegexString += segment.RegexString;

                var shouldAddSpace =
                    !_noSpaces
                    && i < RegexSegments.Count - 1
                    && !(segment is BoolRegexProp)
                    && !TerminalPunctuation.Contains(segment.RegexString);

                if (shouldAddSpace)
                    RegexString += " ";
            }
        }

        RegexStringNoWordBoundaries = RegexString;
        RegexStringNoCaptureGroups = StripNamedCaptureGroups(RegexString);

        if (_containingType.GetCustomAttribute<NoWordBoundaryAttribute>() == null)
        {
            // TokenRegexOneOfProps are avoided, because they handle their own boundaries internally

            if (RegexSegments.First() is not TokenRegexOneOfProp)
                RegexString = $@"\b{RegexString}";

            if (RegexSegments.Last() is not TokenRegexOneOfProp)
                RegexString = $@"{RegexString}\b";
        }

        Regex = new Regex(RegexString, RegexOptions.Compiled);
    }

    RegexSegmentBase ResolveSnippetToPropOrTextSegment(string templateSnippet)
    {
        var matchingProp = RegexPropInfos.FirstOrDefault(x => x.Name == templateSnippet);

        if (matchingProp is not null)
        {
            if (matchingProp.IsManyItem)
                return new TokenRegexManyProp(matchingProp);

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

    List<RegexPropInfo> GetRegexProps()
    {
        static bool IsTarget(Type t)
        {
            var u = Nullable.GetUnderlyingType(t) ?? t;

            if (u.IsGenericType && u.GetGenericTypeDefinition() == typeof(ManyToken<>))
                u = u.GetGenericArguments()[0];

            return u.IsEnum
                || u == typeof(bool)
                || u == typeof(PlaceholderCapture)
                || typeof(TokenUnit).IsAssignableFrom(u);
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


    /// <summary>
    /// Strips all named capture groups from a regex pattern,
    /// leaving the inner pattern within a non-named capture group.
    /// </summary>
    /// <param name="pattern">The input regex pattern.</param>
    /// <returns>A new regex pattern with named capture groups removed.</returns>
    static string StripNamedCaptureGroups(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return pattern;
        }

        var result = new StringBuilder();
        int length = pattern.Length;
        int index = 0;

        while (index < length)
        {
            if (pattern[index] == '(')
            {
                // Potential start of a group
                if (index + 2 < length && pattern[index + 1] == '?' && pattern[index + 2] == '<')
                {
                    // This is a named capture group
                    int groupStartIndex = index;
                    index += 3; // Move past '(?<'

                    // Find the closing '>' of the name
                    int nameEndIndex = pattern.IndexOf('>', index);
                    if (nameEndIndex == -1)
                    {
                        // Invalid pattern, append as is and break
                        result.Append(pattern.Substring(groupStartIndex));
                        break;
                    }

                    index = nameEndIndex + 1;
                    int parenCount = 1;
                    int contentStartIndex = index;

                    // Find the matching closing parenthesis for the named group
                    while (index < length && parenCount > 0)
                    {
                        if (pattern[index] == '\\' && index + 1 < length)
                        {
                            index += 2; // Skip escaped character
                            continue;
                        }

                        if (pattern[index] == '(')
                        {
                            parenCount++;
                        }
                        else if (pattern[index] == ')')
                        {
                            parenCount--;
                        }
                        index++;
                    }

                    if (parenCount == 0)
                    {
                        // Successfully found the closing parenthesis
                        string content = pattern.Substring(contentStartIndex, index - 1 - contentStartIndex);
                        result.Append('(').Append(content).Append(')');
                    }
                    else
                    {
                        // Unterminated named group, append the original substring and stop
                        result.Append(pattern.Substring(groupStartIndex));
                        break;
                    }
                }
                else
                {
                    // Not a named capture group, append the parenthesis and continue
                    result.Append(pattern[index]);
                    index++;
                }
            }
            else if (pattern[index] == '\\' && index + 1 < length)
            {
                // Handle escaped characters
                result.Append(pattern[index]);
                result.Append(pattern[index + 1]);
                index += 2;
            }
            else
            {
                // Any other character
                result.Append(pattern[index]);
                index++;
            }
        }

        return result.ToString();
    }

}