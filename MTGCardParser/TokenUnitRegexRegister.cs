namespace MTGCardParser;

public static class TokenUnitRegexRegister
{
    //public static Dictionary<Type, List<PropertyInfo>> TypeCaptureProps { get; set; } = new();
    public static Dictionary<Type, RegexTemplate> TypeRegexTemplates { get; set; } = new();
    //public static Dictionary<CaptureProp, IPropRegexSegment> PropRegexSegments { get; set; } = new();
    public static Dictionary<Type, Dictionary<object, Regex>> EnumRegexes { get; set; } = new();
    //public static Dictionary<PropertyInfo, List<PropertyInfo>> AlternatePropSets { get; set; } = new();
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
            var regexTemplate = new RegexTemplate(instance.RegexTemplate);
            TypeRegexTemplates[type] = regexTemplate;
            var propCaptureSegments = regexTemplate.PropCaptureSegments;

            //foreach (var propCaptureSegment in propCaptureSegments)
            //    PropRegexSegments[propCaptureSegment.CaptureProp] = propCaptureSegment;

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

        // If implementing class overrides instantiation, return after handling
        if (parentInstance.HandleInstantiation(matchString))
            return parentInstance;
        // Otherwise handle default instantiation
        else
        {
            foreach (var propCaptureSegment in parentInstance.RegexTemplate.PropCaptureSegments)
                propCaptureSegment.SetValueFromMatchString(parentInstance, matchString);

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
                // 1) It’s a concrete class
                t.IsClass && !t.IsAbstract
                // 2) It implements ITokenCapture (directly or indirectly)
                && typeof(ITokenUnit).IsAssignableFrom(t))
            .ToList();


    //static List<PropertyInfo> GetCaptureProps(Type type) => type.GetPropertiesForCapture().ToList();

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

