using MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

namespace MTGPlexer;

public class RegexTemplate
{
    public static HashSet<string> Punctuation = [".", ",", ";", "\""];
    public static HashSet<char> TerminalPunctuation = ['.', ',', ';'];

    Type _containingType;

    public string RegexString { get; private set; }
    public Regex Regex { get; private set; }
    public List<RegexPropInfo> RegexPropInfos { get; private set; } = [];
    public List<RegexSegmentBase> RegexSegments { get; private set; } = [];
    public List<RegexTemplateLine> Lines { get; private set; } = [];
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

        ComposeRegexLines();
    }

    void ComposeRegexLines()
    {
        RegexLineCollector collector = new(_containingType);

        foreach (var segment in RegexSegments)
            segment.ComposeRegexLines(collector);

        Lines = collector.Finalize();
        SetRegex();
    }

    void SetRegex()
    {
        const int spacesPerIndent = 4;
        string regexString = "";

        foreach (var line in Lines)
            regexString += string.Empty.PadLeft(spacesPerIndent * line.Indentation) + line.Value + Environment.NewLine;

        regexString = regexString.Trim();
        RegexString = regexString;
        Regex = new Regex(regexString, RegexOptions.Compiled);
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

//public enum RegexTemplateType
//{
//    TokenUnit,
//    OneOf,
//    Many
//}