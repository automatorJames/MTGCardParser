using MTGPlexer.RegexGeneration.Composers;
using MTGPlexer.RegexGeneration.RegexSegments;

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
        bool neverAddSpacesAtTopLevel = false;
        ISegmentComposer composer;

        if (_containingType.IsAssignableTo(typeof(TokenUnitOneOf)))
        {
            // This is the CRITICAL check for the top-level entity.
            neverAddSpacesAtTopLevel = !RegexSegments.Any(x => x is TextSegment);
            composer = AlternatingComposer.Instance;
        }
        else
        {
            composer = ConcatenatingComposer.Instance;
        }

        // The result of the check is now passed to the collector.
        RegexLineCollector collector = new(_containingType, neverAddSpacesAtTopLevel);
        composer.Compose(collector, RegexSegments);

        GeneratedRegex = collector.Finalize();
        RegexString = GeneratedRegex.MinifiedRegex;
        FormattedRegexString = GeneratedRegex.FormattedRegex;
        MinifiedRegexString = GeneratedRegex.MinifiedRegex;
        Regex = new Regex(GeneratedRegex.MinifiedRegex, RegexOptions.Compiled);
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