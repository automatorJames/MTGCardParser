namespace MTGCardParser;

public static class TypeRegistry
{
    public static Dictionary<Type, List<CaptureProp>> CaptureProps { get; set; } = new();
    public static Dictionary<Type, RegexTemplate> TypeRegexTemplates { get; set; } = new();
    public static Dictionary<PropertyInfo, IRegexSegment> PropRegexPatterns { get; set; } = new();
    public static Dictionary<Type, Dictionary<object, Regex>> EnumRegexes { get; set; } = new();
    public static Tokenizer<Type> Tokenizer { get; set; }
    public static HashSet<Type> AppliedOrderTypes { get; set; } = new();

    public static void Initialize()
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
            var captureProps = GetCaptureProps(type);
            CaptureProps[type] = captureProps;
            var regexTemplate = new RegexTemplate(instance.RegexTemplate);
            TypeRegexTemplates[type] = regexTemplate;

            foreach (var propEntry in regexTemplate.PropRegexSegments)
                PropRegexPatterns[propEntry.Key] = propEntry.Value;

            var unregisteredEnums = regexTemplate.PropRegexSegments
                .Where(x => x.Value is EnumCaptureGroup enumCap && !EnumRegexes.ContainsKey(enumCap.EnumType))
                .Select(x => x.Value as EnumCaptureGroup);

            foreach (var enumEntry in unregisteredEnums)
            {
                var newEnumEntry = EnumRegexes[enumEntry.EnumType] = new();

                foreach (var enumValueEntry in enumEntry.EnumMemberRegexes)
                    newEnumEntry[enumValueEntry.Key] = enumValueEntry.Value;
            }
        }
    }

    public static ITokenCapture HydrateFromToken(Token<Type> token) => InstantiateFromTypeAndMatchString(token.Kind, token.ToStringValue());

    public static ITokenCapture InstantiateFromTypeAndMatchString(Type type, string matchString)
    {
        if (!type.IsAssignableTo(typeof(ITokenCapture)))
            throw new Exception($"{type.Name} does not implement {nameof(ITokenCapture)}");

        var instance = (ITokenCapture)Activator.CreateInstance(type);

        // If implementing class overrides instantiation, return after handling
        if (instance.HandleInstantiation(matchString))
            return instance;
        // Otherwise handle instantiation here
        else
        {
            var typeMatch = Regex.Match(matchString, TypeRegexTemplates[type].RenderedRegex);

            foreach (var captureProp in CaptureProps[type])
                captureProp.SetValue(instance, typeMatch);

            return instance;
        }
    }

    static List<ITokenCapture> GetTokenCaptureInstances() =>
        GetTokenCaptureTypes()
        .Select(x => (ITokenCapture)Activator.CreateInstance(x))
        .ToList();

    static List<Type> GetTokenCaptureTypes() =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                // 1) It’s a concrete class
                t.IsClass && !t.IsAbstract
                // 2) It implements ITokenCapture (directly or indirectly)
                && typeof(ITokenCapture).IsAssignableFrom(t))
            .ToList();


    static List<CaptureProp> GetCaptureProps(Type type) => 
        type.GetPropertiesForCapture()
        .Select(x => new CaptureProp(x))
        .ToList();

    static void InitializeTokenizer()
    {
        var tokenizerBuilder = new TokenizerBuilder<Type>();

        tokenizerBuilder.Ignore(Span.Regex(@"[ \t]+"));

        tokenizerBuilder
            .Match(typeof(This))
            .Match(typeof(ActivatedAbility))
            .Match(typeof(LoseOrGainAbility))
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

        tokenizerBuilder
            .Match(@"[^.,;""\s]+");

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

