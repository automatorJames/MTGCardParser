using System.Linq;

namespace MTGPlexer.TokenUnitComponents;

public class RegexTemplate
{
    public static HashSet<string> Punctuation = [".", ",", ";", "\""];
    public static HashSet<char> TerminalPunctuation = ['.', ',', ';'];

    Type _containingType;

    public string RegexString { get; private set; }
    public Regex Regex { get; private set; }
    public RegexBuilder Builder { get; private set; }
    public List<TemplatePropInfo> TemplatePropInfos { get; private set; } = [];
    public List<RegexSegmentBase> RegexSegments { get; private set; } = [];
    public List<CaptureGroupSegmentBase> CaptureGroupProps => RegexSegments.OfType<CaptureGroupSegmentBase>().ToList();

    public RegexTemplate(Type type)
    {
        var instance = Activator.CreateInstance(type);

        if (instance is not TokenUnit tokenUnitInstance)
            throw new Exception($"Type '{type.Name}' does not derive from type '{nameof(TokenUnit)}'");

        var snippets = tokenUnitInstance.GetSnippets();

        if (snippets.Length == 0)
        {
            // If children pass no arguments or call the default parameterless base constructor,
            // we assume they want to construct snippets from their ordered properties. If no
            // properties exist, we assume they want to construct a single snippet from a pattern attribute,
            // or even the type name as a last-ditch fallback.

            var publicPropNames = type.GetPublicPropNames();

            if (publicPropNames.Length > 0)
                snippets = type.GetPublicPropNames().Select(x => (Snippet)x).ToArray();
            else if (type.GetCustomAttribute<RegexPatternAttribute>() is RegexPatternAttribute attr)
                snippets = attr.Patterns.Select(x => (Snippet)x).ToArray();
            else
                snippets = [type.Name.ToFriendlyCase(TitleDisplayOption.Lower)];

            if (snippets.Length == 0)
                throw new Exception($"Type '{type.Name}' has no snippets or valid properties");
        }

        _containingType = type;
        TemplatePropInfos = GetTemplateProps();

        for (int i = 0; i < snippets.Length; i++)
        {
            var snippet = snippets[i];
            var segment = ResolveSnippetToSegment(snippet);
            RegexSegments.Add(segment);
        }

        ComposeRegex();
    }

    /// <summary>
    /// Takes a string like "these are my template @Type snippets with interspersed Opt(snippet shortcuts)", 
    /// converts them into a set of RegexSegment bases, then uses the appropriate Composer strategy to generate 
    /// a regex string. Used for rendering templated regex patterns for preview without commiting emitted types to the runtime.
    /// </summary>
    /// <typeparam name="T">The type of TokenUnit or derivative used to dermine the Composer strategy</typeparam>
    /// <param name="templateString">a string of words containing "@Token" style type tokens</param>
    public static string TemplateStringToRegex<T>(string templateString) where T : TokenUnit
    {
        // Pattern to split on both "@Token" elements and "Opt(text)" elements interspered in normal text
        var splittingPattern = @"(@\w+|(?:Alt|Opt|NoSpace|Prop)\([^)]+\)|.+?(?=@\w+|(?:Alt|Opt|NoSpace|Prop)\(|$))";

        var snippets = Regex.Split(templateString, splittingPattern)
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.Trim());

        List<RegexSegmentBase> regexSegments = snippets.Select<string, RegexSegmentBase>(x =>
        {
            // Case A: It's a Type Token (Check if it starts with @ and try to resolve)
            if (x.StartsWith("@") && TokenTypeRegistry.NameToType.TryGetValue(x[1..], out var type))
                return new TemplatePropInfo(type).GetCaptureGroupPropBase();

            // Case B: It's a Shortcut Token
            var shortcutMatch = Regex.Match(x, @"^(?<Name>Alt|Opt|NoSpace|Prop)\((?<Args>.+)\)$");
            if (shortcutMatch.Success)
            {
                var methodName = shortcutMatch.Groups["Name"].Value;
                var argsString = shortcutMatch.Groups["Args"].Value;

                var method = typeof(SnippetShortcuts).GetMethod(methodName);
                if (method != null)
                {
                    var parameters = method.GetParameters();
                    object[] invokeArgs = parameters.Length switch
                    {
                        1 when parameters[0].ParameterType == typeof(string[])
                            => new object[] { argsString.Split(',').Select(s => s.Trim()).ToArray() },
                        1 => new object[] { argsString },
                        2 => new object[] { null, argsString },
                        _ => null
                    };

                    if (invokeArgs != null)
                    {
                        var shortcutSnippet = (Snippet)method.Invoke(null, invokeArgs);
                        return new TextSegment(shortcutSnippet);
                    }
                }
            }

            // Case C: Plain text (including literal spaces from the template)
            return new TextSegment(x);
        }).ToList();

        return CompositionFactory.GetComposedString(regexSegments, typeof(T));
    }

    void ComposeRegex()
    {
        var builderWithComposition = CompositionFactory.Compose(RegexSegments, _containingType);
        RegexString = builderWithComposition.GetMinified();
        Regex = new Regex(RegexString, RegexOptions.Compiled);
        Builder = builderWithComposition;
    }

    RegexSegmentBase ResolveSnippetToSegment(Snippet templateSnippet)
    {
        var matchingProp = TemplatePropInfos.FirstOrDefault(x => x.Prop.Name == templateSnippet);

        if (matchingProp != null)
            return matchingProp.GetCaptureGroupPropBase();
        else
            return new TextSegment(templateSnippet);
    }

    List<TemplatePropInfo> GetTemplateProps()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        return _containingType
            .GetProperties(flags)
            .Where(p => p.GetMethod is { IsVirtual: false }) // Must be non-virtual
            .Where(p => IsValidTargetType(p.PropertyType))
            .Select(p => new TemplatePropInfo(p))
            .ToList();
    }

    static bool IsValidTargetType(Type type)
    {
        // Unwrap Nullable
        type = Nullable.GetUnderlyingType(type) ?? type;

        // Handle Generic Wrappers
        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();

            // Single-argument wrappers: ManyOf<T> or CompoundOf<T>
            if (genericDef == typeof(ManyOf<>) || genericDef == typeof(CompoundOf<>) || genericDef == typeof(OptionalOf<>))
                return IsValidTargetType(type.GetGenericArguments()[0]);

            // Multi-argument wrappers: OneOf<T1, T2> or OneOf<T1, T2, T3>
            if (genericDef == typeof(OneOf<,>) || genericDef == typeof(OneOf<,,>))
                return type.GetGenericArguments().All(IsValidTargetType);

        }

        // Check Leaf/Base Types
        return IsLeafTargetType(type);
    }

    static bool IsLeafTargetType(Type type)
    {
        return type.IsEnum
            || type == typeof(bool)
            || type == typeof(PlaceholderCapture)
            || type.IsAssignableTo(typeof(DynamicOf))
            || typeof(TokenUnit).IsAssignableFrom(type);
    }
}