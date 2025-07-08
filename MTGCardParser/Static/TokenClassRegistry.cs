namespace MTGCardParser.Static;

public static class TokenClassRegistry
{
    private static bool _isInitialized;
    public static bool IsInitialized => _isInitialized;

    public static Dictionary<Type, RegexTemplate> TypeRegexTemplates { get; set; } = new();
    public static Dictionary<Type, Regex> TypeRegexes { get; set; } = new();

    public static Tokenizer<Type> Tokenizer { get; set; }
    public static HashSet<Type> AppliedOrderTypes { get; set; } = new();

    static TokenClassRegistry()
    {
        RegisterTypes();
        BakeAllRegexes();
        InitializeTokenizer();
        _isInitialized = true;
    }

    static void RegisterTypes()
    {
        var instances = GetTokenCaptureInstances();
        foreach (var instance in instances)
        {
            var type = instance.GetType();
            var regexTemplate = instance.GetRegexTemplate();
            TypeRegexTemplates[type] = new RegexTemplate(regexTemplate);
        }
    }

    private static void BakeAllRegexes()
    {
        foreach (var kvp in TypeRegexTemplates)
        {
            var tokenType = kvp.Key;
            var template = kvp.Value;
            var renderedString = template.RenderedRegexString;

            // CORRECTED: Remove RegexOptions.IgnoreCase, as patterns are now manually lowercased.
            TypeRegexes[tokenType] = new Regex(renderedString, RegexOptions.Singleline | RegexOptions.Compiled);
        }
    }

    public static TokenUnit HydrateFromToken(Token<Type> token)
        => TokenUnit.InstantiateFromMatchString(token.Kind, token.Span);

    static List<TokenUnit> GetTokenCaptureInstances() =>
        GetTokenCaptureTypes()
        .Select(x => (TokenUnit)Activator.CreateInstance(x))
        .ToList();

    static List<Type> GetTokenCaptureTypes() =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass && !t.IsAbstract
                && typeof(TokenUnit).IsAssignableFrom(t))
            .ToList();

    static void InitializeTokenizer()
    {
        var tokenizerBuilder = new TokenizerBuilder<Type>();
        tokenizerBuilder.Ignore(Span.Regex(@"[ \t]+"));

        tokenizerBuilder
            .Match(typeof(This))
            .Match(typeof(ActivatedAbility))
            .Match(typeof(YouMayPayCost))
            .Match(typeof(GainOrLoseAbility))
            .Match(typeof(EnchantedCard))
            .Match(typeof(EnchantCard))
            .Match(typeof(CardKeyword))
            .Match(typeof(AtOrUntilPlayerPhase))
            .Match(typeof(IfYouDo))
            .Match(typeof(LifeChangeQuantity))
            .Match(typeof(Punctuation))
            .Match(typeof(ManaValue))
            .Match(typeof(Parenthetical));

        foreach (var key in TypeRegexTemplates.Keys)
            if (!AppliedOrderTypes.Contains(key))
                tokenizerBuilder.Match(key);

        tokenizerBuilder.Match(typeof(DefaultUnmatchedString));

        Tokenizer = tokenizerBuilder.Build();
    }

    public static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, Type tokenCaptureType)
    {
        if (AppliedOrderTypes.Contains(tokenCaptureType))
            return tokenizerBuilder;

        // CORRECTED: Remove RegexOptions.IgnoreCase here as well. The tokenizer should match the pre-lowercased patterns.
        // The tokenizer also lowercases the input text by default, making this match work.
        tokenizerBuilder.Match(Span.Regex(TypeRegexTemplates[tokenCaptureType].RenderedRegexString), tokenCaptureType);
        AppliedOrderTypes.Add(tokenCaptureType);

        return tokenizerBuilder;
    }
}