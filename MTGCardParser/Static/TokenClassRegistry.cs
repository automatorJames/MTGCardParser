namespace MTGCardParser.Static;

public static class TokenClassRegistry
{
    public static Dictionary<Type, RegexTemplate> TypeRegexTemplates { get; set; } = new();
    public static Dictionary<Type, Regex> TypeRegexes { get; set; } = new();
    public static Dictionary<Type, Dictionary<object, Regex>> EnumRegexes { get; set; } = new();
    public static Tokenizer<Type> Tokenizer { get; set; }
    public static HashSet<Type> AppliedOrderTypes { get; set; } = new();

    static TokenClassRegistry()
    {
        RegisterTypes();
        InitializeTokenizer();
    }

    static void RegisterTypes()
    {
        foreach (var type in GetTokenCaptureTypes())
            SetTypeTemplate(type);
    }

    public static Regex GetTypeRegex(Type type)
    {
        if (!TypeRegexes.ContainsKey(type))
            SetTypeTemplate(type);

        return TypeRegexes[type];
    }

    public static RegexTemplate GetTypeTemplate(Type type)
    {
        if (!TypeRegexTemplates.ContainsKey(type))
            SetTypeTemplate(type);

        return TypeRegexTemplates[type];
    }

    public static void SetTypeTemplate(Type type)
    {
        var instance = (TokenUnit)Activator.CreateInstance(type);
        TypeRegexTemplates[type] = instance.Template;
        TypeRegexes[type] = new Regex(instance.Template.RenderedRegexString, RegexOptions.Compiled);
        var propCaptureSegments = instance.Template.PropCaptureSegments;

        var unregisteredEnums = propCaptureSegments
            .OfType<EnumRegexProp>()
            .Where(x => !EnumRegexes.ContainsKey(x.RegexPropInfo.UnderlyingType));

        foreach (var enumEntry in unregisteredEnums)
            EnumRegexes[enumEntry.RegexPropInfo.UnderlyingType] = enumEntry.EnumMemberRegexes;
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

    static void InitializeTokenizer()
    {
        var tokenizerBuilder = new TokenizerBuilder<Type>();
        tokenizerBuilder.Ignore(Span.Regex(@"[ \t]+"));

        tokenizerBuilder
            .Match(typeof(This))
            .Match(typeof(ActivatedAbility))
            .Match(typeof(OptionalPayCost))
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

