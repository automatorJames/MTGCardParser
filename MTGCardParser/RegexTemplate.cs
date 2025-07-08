using System.Text;

namespace MTGCardParser.TokenTesting;

public class RegexTemplate
{
    // This is a direct copy from your original code to ensure exact behavior.
    static readonly HashSet<string> TerminalPunctuation = [".", ",", ";"];

    private string _lazyRenderedRegexString;
    public string RenderedRegexString
    {
        get
        {
            if (_lazyRenderedRegexString == null)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < _templateSnippets.Length; i++)
                {
                    var snippet = _templateSnippets[i];
                    string regexPart;
                    PropertyCapture capture = null;

                    if (snippet is string propName && _propCaptureDict.TryGetValue(propName, out capture))
                    {
                        regexPart = capture.RegexPattern;
                    }
                    else if (snippet is string literal)
                    {
                        regexPart = literal.ToLower();
                    }
                    else
                    {
                        // Should not happen with current logic
                        continue;
                    }

                    sb.Append(regexPart);

                    // Re-implementing the original space-handling logic precisely
                    var isLastElement = i == _templateSnippets.Length - 1;
                    var isBoolCapture = capture is BoolPropertyCapture;
                    var isTerminalPunctuation = TerminalPunctuation.Contains(regexPart);

                    var shouldAddSpace = !_noSpaces && !isLastElement && !isBoolCapture && !isTerminalPunctuation;

                    if (shouldAddSpace)
                    {
                        // Using \s+ is more robust than a literal space
                        sb.Append(@"\s+");
                    }
                }
                _lazyRenderedRegexString = sb.ToString();
            }
            return _lazyRenderedRegexString;
        }
    }

    public IReadOnlyList<PropertyCapture> PropertyCaptures { get; }

    private readonly object[] _templateSnippets;
    private readonly bool _noSpaces;
    private readonly Dictionary<string, PropertyCapture> _propCaptureDict;

    protected RegexTemplate(Type tokenUnitType, object[] templateSnippets)
    {
        _templateSnippets = templateSnippets;
        _noSpaces = tokenUnitType.GetCustomAttribute<NoSpacesAttribute>() is not null;

        var properties = tokenUnitType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(p => !p.GetMethod.IsVirtual)
            .Where(PropertyCapture.IsValidCaptureProperty);

        PropertyCaptures = properties
            .Select(PropertyCapture.Create)
            .ToList();

        _propCaptureDict = PropertyCaptures.ToDictionary(p => p.Prop.Name);
    }

    public RegexTemplate(Type enumType)
    {
        _templateSnippets = [];
        PropertyCaptures = [];
        _propCaptureDict = new();
        _noSpaces = false;

        var memberPatterns = Enum.GetNames(enumType)
            .Select(name =>
            {
                var memberInfo = enumType.GetMember(name).First();
                var attribute = memberInfo.GetCustomAttribute<RegexPatternAttribute>();
                return attribute?.Patterns.First() ?? name;
            });

        _lazyRenderedRegexString = $"(?:{string.Join("|", memberPatterns)})";
    }

    public RegexTemplate(RegexTemplate source)
    {
        PropertyCaptures = source.PropertyCaptures;
        _templateSnippets = source._templateSnippets;
        _noSpaces = source._noSpaces;
        _propCaptureDict = source._propCaptureDict;
    }

    public void HydrateInstance(TokenUnit instance, Match match)
    {
        foreach (var capture in PropertyCaptures)
        {
            capture.HydrateProperty(instance, match);
        }
    }
}

public class RegexTemplate<T> : RegexTemplate where T : TokenUnit
{
    public RegexTemplate(params object[] templateSnippets)
        : base(typeof(T), templateSnippets)
    {
    }
}