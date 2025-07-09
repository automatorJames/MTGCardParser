namespace MTGCardParser.Static;

public static class TokenClassRegistry
{
    private static bool _isInitialized;
    public static bool IsInitialized => _isInitialized;

    public static Dictionary<Type, RegexTemplate> TypeRegexTemplates { get; set; } = new();
    public static Dictionary<Type, Regex> TypeRegexes { get; set; } = new();
    public static Dictionary<Type, Dictionary<object, Regex>> EnumRegexes { get; set; } = new();
    public static Tokenizer<Type> Tokenizer { get; set; }
    public static HashSet<Type> AppliedOrderTypes { get; set; } = new();

    static TokenClassRegistry()
    {
        RegisterTypes();
        InitializeTokenizer();
        _isInitialized = true;
    }

    static void RegisterTypes()
    {
        var instances = GetTokenCaptureInstances();
        foreach (var instance in instances)
        {
            var type = instance.GetType();
            var regexTemplate = new RegexTemplate(instance.GetRegexTemplate());
            TypeRegexTemplates[type] = regexTemplate;
            TypeRegexes[type] = new Regex(regexTemplate.RenderedRegexString, RegexOptions.Compiled);
            var propCaptureSegments = regexTemplate.PropCaptureSegments;

            var unregisteredEnums = propCaptureSegments
                .OfType<EnumRegexProp>()
                .Where(x => !EnumRegexes.ContainsKey(x.RegexPropInfo.UnderlyingType));

            foreach (var enumEntry in unregisteredEnums)
                EnumRegexes[enumEntry.RegexPropInfo.UnderlyingType] = enumEntry.EnumMemberRegexes;
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
            .Match(typeof(EnchantCard))
            .Match(typeof(CardKeyword))
            .Match(typeof(AtOrUntilPlayerPhase))
            .Match(typeof(IfYouDo))
            .Match(typeof(EnchantedCard))
            .Match(typeof(LifeChangeQuantity))
            .Match(typeof(Punctuation))
            .Match(typeof(ManaValue))
            .Match(typeof(Parenthetical));
        
        // Apply assembly types that weren't applied above (failsafe for laziness)
        foreach (var key in TypeRegexTemplates.Keys)
            if (!AppliedOrderTypes.Contains(key))
                tokenizerBuilder.Match(key);

        // Catch anything else with the default string pattern
        tokenizerBuilder.Match(typeof(DefaultUnmatchedString));

        //tokenizerBuilder
        //    .Match(@"[^.,;""\s]+");

        Tokenizer = tokenizerBuilder.Build();
    }

    public static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, Type tokenCaptureType)
    {
        if (AppliedOrderTypes.Contains(tokenCaptureType))
            return tokenizerBuilder;

        tokenizerBuilder.Match(Span.Regex(TypeRegexTemplates[tokenCaptureType].RenderedRegexString), tokenCaptureType);
        AppliedOrderTypes.Add(tokenCaptureType);

        return tokenizerBuilder;
    }
}

