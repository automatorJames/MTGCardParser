namespace MTGCardParser.Static;

public static class TokenClassRegistry
{
    public static Dictionary<Type, RegexTemplate> TypeRegexTemplates { get; set; } = new();
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
            var regexTemplate = new RegexTemplate(instance.RegexTemplate);
            TypeRegexTemplates[type] = regexTemplate;
            var propCaptureSegments = regexTemplate.PropCaptureSegments;

            var unregisteredEnums = propCaptureSegments
                .OfType<EnumCaptureSegment>()
                .Where(x => !EnumRegexes.ContainsKey(x.CaptureProp.UnderlyingType));

            foreach (var enumEntry in unregisteredEnums)
                EnumRegexes[enumEntry.CaptureProp.UnderlyingType] = enumEntry.EnumMemberRegexes;
        }
    }

    public static ITokenUnit HydrateFromToken(Token<Type> token) => InstantiateFromTypeAndMatchString(token.Kind, token.ToStringValue());

    public static ITokenUnit InstantiateFromTypeAndMatchString(Type type, string matchString)
    {
        if (!type.IsAssignableTo(typeof(ITokenUnit)))
            throw new Exception($"{type.Name} does not implement {nameof(ITokenUnit)}");

        var parentInstance = (ITokenUnit)Activator.CreateInstance(type);

        if (parentInstance.HandleInstantiation(matchString))
            // If implementing class overrides instantiation, return after handling
            return parentInstance;
        else
        {
            // Otherwise handle default instantiation
            foreach (var propCaptureSegment in parentInstance.RegexTemplate.PropCaptureSegments)
                propCaptureSegment.SetValueFromMatchString(parentInstance, matchString);

            foreach (var alternativeCaptureSet in parentInstance.RegexTemplate.AlternativePropCaptureSets)
            {
                // .Any() is used here as a short-circuiting evaluator with the side effect of setting the value on the instance
                bool matchFound = alternativeCaptureSet.Alternatives.Any(x => x.SetValueFromMatchString(parentInstance, matchString));

                if (!matchFound)
                    throw new Exception($"Match string '{matchString}' was passed to an alternative set, but no alternative was matched");
            }

            return parentInstance;
        }
    }

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

        tokenizerBuilder.Match(Span.Regex(TypeRegexTemplates[tokenCaptureType].RenderedRegex), tokenCaptureType);
        AppliedOrderTypes.Add(tokenCaptureType);

        return tokenizerBuilder;
    }

    public static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, string regexPattern, bool wrapInWordboundary = true)
    {
        tokenizerBuilder.Match(Span.Regex(regexPattern), typeof(string));
        return tokenizerBuilder;
    }
}

