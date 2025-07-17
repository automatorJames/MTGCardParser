namespace MTGPlexer.Static;

public static partial class TokenTypeRegistry
{
    static HashSet<Type> _invalidTypes = [];

    public static Dictionary<Type, RegexTemplate> Templates { get; set; } = [];
    public static Dictionary<string, Type> NameToType { get; set; } = [];
    public static Dictionary<Type, Dictionary<object, Regex>> EnumRegexes { get; set; } = [];
    public static Dictionary<Type, Dictionary<PropertyInfo, List<PropertyInfo>>> DistilledProperties { get; set; } = [];
    public static Dictionary<Type, DeterministicPalette> TypeColorPalettes { get; set; } = [];
    public static HashSet<Type> AppliedOrderTypes { get; set; } = [];
    public static HashSet<Type> ReferencedEnumTypes { get; set; } = [];
    public static Tokenizer<Type> Tokenizer { get; set; }

    static TokenTypeRegistry()
    {
        ValidateAndRegisterTypes();
        InitializeTokenizer();
    }

    static void ValidateAndRegisterTypes()
    {
        foreach (var type in GetTokenCaptureTypes())
            SetTypeTemplate(type);

        ReferencedEnumTypes = EnumRegexes.Keys.ToHashSet();
    }

    public static RegexTemplate GetTypeTemplate(Type type)
    {
        if (!Templates.ContainsKey(type))
            SetTypeTemplate(type);

        return Templates[type];
    }

    public static void SetTypeTemplate(Type type)
    {
        var instance = (TokenUnit)Activator.CreateInstance(type);

        if (!instance.ValidateStructure())
        {
            _invalidTypes.Add(type);
            return;
        }

        Templates[type] = instance.Template;
        NameToType[type.Name] = type;
        var propCaptureSegments = instance.Template.PropCaptureSegments;

        var unregisteredEnums = propCaptureSegments
            .OfType<EnumRegexProp>()
            .Where(x => !EnumRegexes.ContainsKey(x.RegexPropInfo.UnderlyingType));

        foreach (var enumEntry in unregisteredEnums)
            EnumRegexes[enumEntry.RegexPropInfo.UnderlyingType] = enumEntry.EnumMemberRegexes;

        TypeColorPalettes[type] = new(type);

        if (instance is TokenUnitDistilled tokenUnitComplex)
        {
            DistilledProperties[type] = new();

            foreach (var item in tokenUnitComplex.GetDistilledPropAssociations())
                DistilledProperties[type][item.Key] = item.Value;
        }
    }

    public static string RenderTemplateToRegexString(string templateString)
    {
        var templateReplacement = Regex.Replace(templateString, @"\@(?<TypeName>\w+)\b", match =>
        {
            var typeName = match.Groups["TypeName"].Value;
            var type = NameToType[typeName];

            if (type != null && Templates.TryGetValue(type, out var renderedTemplateSnippet))
                return renderedTemplateSnippet.RenderedRegexString;

            return match.Value; // fallback: leave original
        });

        return templateReplacement;
    }

    public static List<Token<Type>> TokenizeAndCoallesceUnmatched(string text)
    {
        List<Token<Type>> coallescedTokens = [];
        List<Token<Type>> unmatchedBuffer = [];
        var tokens = Tokenizer.Tokenize(text);

        foreach (var token in tokens)
        {
            if (token.Kind == typeof(DefaultUnmatchedString))
                unmatchedBuffer.Add(token);
            else
            {
                // flush the buffer and append
                FlushBuffer();
                coallescedTokens.Add(token);
            }
        }

        FlushBuffer();

        // local helper
        void FlushBuffer()
        {
            if (unmatchedBuffer.Count > 0)
            {
                Token<Type> combinedUnmatchedStringToken = default;

                if (unmatchedBuffer.Count > 1)
                    combinedUnmatchedStringToken = CoallesceUnmatchedStringTokens(unmatchedBuffer);
                else if (unmatchedBuffer.Count == 1)
                    combinedUnmatchedStringToken = unmatchedBuffer[0];

                coallescedTokens.Add(combinedUnmatchedStringToken);
            }

            unmatchedBuffer = [];
        }

        return coallescedTokens;
    }

    static Token<Type> CoallesceUnmatchedStringTokens(List<Token<Type>> unmatchedStringTokens)
    {
        var originalSource = unmatchedStringTokens[0].Span;
        var firstItem = unmatchedStringTokens[0];
        var lastItem = unmatchedStringTokens[^1];
        var start = firstItem.Span.Position.Absolute;
        var combinedLength = lastItem.Span.Position.Absolute + lastItem.Span.Length - start;
        var position = new Position(firstItem.Span.Position.Absolute, firstItem.Span.Position.Line, firstItem.Span.Position.Line);
        var combinedTextSpan = new TextSpan(originalSource.Source, position, combinedLength);
        var token = new Token<Type>(typeof(DefaultUnmatchedString), combinedTextSpan);

        return token;
    }

    public static TokenUnit HydrateFromToken(Token<Type> token) 
        => TokenUnit.InstantiateFromMatchString(token.Kind, token.Span);

    static List<Type> GetTokenCaptureTypes() =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass && !t.IsAbstract
                && typeof(TokenUnit).IsAssignableFrom(t))
            .ToList();

}

