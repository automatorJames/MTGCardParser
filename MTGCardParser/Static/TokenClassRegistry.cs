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
        var instances = GetTokenCaptureInstances();
        foreach (var instance in instances)
        {
            var type = instance.GetType();
            var regexTemplate = new RegexTemplate(instance.GetRegexTemplate());
            TypeRegexTemplates[type] = regexTemplate;
            TypeRegexes[type] = new Regex(regexTemplate.RenderedRegexString, RegexOptions.Compiled);
            var propCaptureSegments = regexTemplate.PropCaptureSegments;

            var unregisteredEnums = propCaptureSegments
                .OfType<EnumCaptureSegment>()
                .Where(x => !EnumRegexes.ContainsKey(x.CaptureProp.UnderlyingType));

            foreach (var enumEntry in unregisteredEnums)
                EnumRegexes[enumEntry.CaptureProp.UnderlyingType] = enumEntry.EnumMemberRegexes;
        }
    }

    public static ITokenUnit HydrateFromToken(Token<Type> token) 
        => TokenUnitBase.InstantiateFromMatchString(token.Kind, token.Span);

    static List<ITokenUnit> GetTokenCaptureInstances() =>
        GetTokenCaptureTypes()
        .Select(x => (ITokenUnit)Activator.CreateInstance(x))
        .ToList();

    static List<Type> GetTokenCaptureTypes() =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass && !t.IsAbstract
                && typeof(ITokenUnit).IsAssignableFrom(t))
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

    public static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, string regexPattern, bool wrapInWordboundary = true)
    {
        tokenizerBuilder.Match(Span.Regex(regexPattern), typeof(string));
        return tokenizerBuilder;
    }
}

