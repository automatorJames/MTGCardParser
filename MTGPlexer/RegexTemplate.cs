namespace MTGPlexer;

public class RegexTemplate
{
    public static HashSet<string> Punctuation = [".", ",", ";", "\""];
    public static HashSet<char> TerminalPunctuation = ['.', ',', ';'];

    Type _containingType;

    public string FormattedRegexString { get; private set; }
    public string MinifiedRegexString { get; private set; }
    public string RegexString { get; private set; }
    public Regex Regex { get; private set; }
    public List<RegexPropInfo> RegexPropInfos { get; private set; } = [];
    public List<RegexSegmentBase> RegexSegments { get; private set; } = [];
    public GeneratedRegex GeneratedRegex { get; private set; }
    public List<CaptureGroupPropBase> CaptureGroupProps => RegexSegments.OfType<CaptureGroupPropBase>().ToList();

    public RegexTemplate(Type type, params string[] templateSnippets)
    {
        if (templateSnippets is null || templateSnippets.Length == 0)
            return;

        _containingType = type;
        RegexPropInfos = GetRegexProps();

        for (int i = 0; i < templateSnippets.Length; i++)
        {
            var snippet = templateSnippets[i];
            var isLastSnippet = i == templateSnippets.Length - 1;
            var segment = ResolveSnippetToSegment(snippet);
            RegexSegments.Add(segment);
        }

        ComposeRegex();
    }

    void ComposeRegex()
    {
        RegexLineCollector collector = new(_containingType);

        if (_containingType.IsAssignableTo(typeof(TokenUnitOneOf)))
            ComposeTokenUnitOneOfLines(collector, RegexSegments);
        else
            ComposeTokenUnitLines(collector, RegexSegments);

        GeneratedRegex = collector.Finalize();
        RegexString = GeneratedRegex.MinifiedRegex;
        FormattedRegexString = GeneratedRegex.FormattedRegex;
        MinifiedRegexString = GeneratedRegex.MinifiedRegex;
        Regex = new Regex(GeneratedRegex.MinifiedRegex, RegexOptions.Compiled);
    }

    public static void ComposeTokenUnitLines(RegexLineCollector collector, List<RegexSegmentBase> segments)
    {
        foreach (var segment in segments)
            segment.ComposeRegexLines(collector);
    }

    public static void ComposeTokenUnitOneOfLines(RegexLineCollector collector, List<RegexSegmentBase> segments)
    {
        // If there are no text segments, the named group parentheses are a sufficient wrapper to isolate
        // the alterantive properties. If not, we must render the alternate properties within supplemental
        // parentheses to isolate them from the text segments on either side.
        bool shouldWrapAlternatives = segments.Any(x => x is TextSegment);

        // Tracks the number of alternatives that have been rendered to open/close groups and render "|" pipes
        int renderedAlternatives = 0;

        foreach (var segment in segments)
        {
            if (segment is TextSegment)
            {
                if (renderedAlternatives > 0)
                    // Close the alternations group before the trailing text segments
                    collector.CloseGroup();

                segment.ComposeRegexLines(collector);

            }
            else if (segment is CaptureGroupPropBase captureProp)
            {
                if (renderedAlternatives == 0 && shouldWrapAlternatives)
                    collector.OpenGroup(neverAddSpacesToGroupMembers: true);

                if (renderedAlternatives > 0)
                    collector.AddGroupAlternativePipe();

                segment.ComposeRegexLines(collector);
                renderedAlternatives++;
            }
        }

        if (shouldWrapAlternatives && renderedAlternatives > 0)
            // Close the alternations group because we're done
            collector.CloseGroup();
    }

    RegexSegmentBase ResolveSnippetToSegment(string templateSnippet)
    {
        var matchingProp = RegexPropInfos.FirstOrDefault(x => x.Name == templateSnippet);

        if (matchingProp != null)
            return matchingProp.GetCaptureGroupPropBase();
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
}