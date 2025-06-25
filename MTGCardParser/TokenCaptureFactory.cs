namespace MTGCardParser;

public static class TokenCaptureFactory
{
    static readonly Tokenizer<Type> _tokenizer;
    static readonly Dictionary<Type, string> _regexTemplates = new();
    static readonly Dictionary<Type, string> _renderedRegexes = new();

    public static readonly HashSet<Type> AppliedOrderTypes = new();

    static TokenCaptureFactory()
    {
        RegisterRegexTemplates();
        _tokenizer = GetTokenizer();
    }

    static void RegisterRegexTemplates()
    {
        foreach (var type in GetTokenCaptureTypes())
        {
            _regexTemplates.Add(type, type.GetRegexTemplate());
            _renderedRegexes.Add(type, type.GetRenderedRegex());
        }
    }

    public static string GetRegexTemplate(Type type)
    {
        if (!_regexTemplates.ContainsKey(type))
            throw new Exception($"No regex template exists for type {type.Name}");

        return _regexTemplates[type];
    }

    public static string GetRenderedRegex(Type type)
    {
        if (!_renderedRegexes.ContainsKey(type))
            throw new Exception($"No rendered regex exists for type {type.Name}");

        return _renderedRegexes[type];
    }

    public static TokenList<Type> Tokenize(string text)
    {
        return _tokenizer.Tokenize(text);
    }

    static List<Type> GetTokenCaptureTypes() =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                // 1) It’s a concrete class
                t.IsClass && !t.IsAbstract
                // 2) It implements ITokenCapture (directly or indirectly)
                && typeof(ITokenCapture).IsAssignableFrom(t))
            .ToList();

    public static Tokenizer<Type> GetTokenizer()
    {
        var tokenizerBuilder = new TokenizerBuilder<Type>();

        tokenizerBuilder.Ignore(Span.Regex(@"[ \t]+"));

        tokenizerBuilder
            .Match(typeof(This))
            .Match(typeof(LoseOrGainAbility))
            .Match(typeof(ActivatedAbility))
            .Match(typeof(EnchantCard))
            .Match(typeof(CardKeyword))
            .Match(typeof(AtOrUntilPlayerPhase))
            .Match(typeof(IfYouDo))
            .Match(typeof(EnchantedCard))
            .Match(typeof(LifeChangeQuantity))
            .Match(typeof(ManaValue))
            .Match(typeof(Punctuation))
            .Match(typeof(Parenthetical));

        // Apply assembly types that weren't applied above (failsafe for laziness)
        foreach (var key in _regexTemplates.Keys)
            if (!AppliedOrderTypes.Contains(key))
                tokenizerBuilder.Match(key);

        tokenizerBuilder
            .Match(@"[^.,;""\s]+");

        return tokenizerBuilder.Build();


        //return new TokenizerBuilder<MtgToken>()
        //.Ignore(Span.Regex(@"[ \t]+"))
        ////.PatternMatch(MtgToken.Newline, "\n", wrapInWordboundary: false)
        ////.PatternMatch(MtgToken.Period, ".", wrapInWordboundary: false)
        //.PatternMatch(MtgToken.This, @"\{this\}", wrapInWordboundary: false)
        //.PatternMatch(MtgToken.Target, "target(s|ed)?")
        //.PatternMatch(MtgToken.Until, "until")
        //.PatternMatch(MtgToken.Tap, @"\{t\}", wrapInWordboundary: false)
        //.PatternMatch(MtgToken.X, "x")
        //.PatternMatch(MtgToken.Destroy, "destroy(s|ed)?")
        //.PatternMatch(MtgToken.ThatDamage, "that damage")
        ////.PatternMatch(MtgToken.PayCost)
        ////.PatternMatch(MtgToken.Color)
        ////.PatternMatch(MtgToken.EnchantCardType)
        ////.PatternMatch(MtgToken.CardType)
        ////.PatternMatch(MtgToken.SubType)
        ////.PatternMatch(MtgToken.CardQuantityChange)
        ////.PatternMatch(MtgToken.Keyword)
        ////.PatternMatch(MtgToken.PlusMinusPowerToughness)
        ////.PatternMatch(MtgToken.AddMana)
        ////.PatternMatch(MtgToken.ManaCost)
        ////.PatternMatch(MtgToken.GamePhase)
        ////.PatternMatch(MtgToken.Who)
        ////.PatternMatch(MtgToken.DealDamageAmount)
        ////.PatternMatch(MtgToken.NextDamageAmount)
        ////.PatternMatch(MtgToken.Quantity)
        //.PatternMatch(MtgToken.When, "when(ever)?")
        //.PatternMatch(MtgToken.Has, "(has|had|have)")
        //.PatternMatch(MtgToken.Get, "get(s)?")
        //.PatternMatch(MtgToken.From, "from")
        //.PatternMatch(MtgToken.May, "may")
        //.PatternMatch(MtgToken.DamageToAny, "damage to any")
        //.PatternMatch(MtgToken.IfYouDo, "if you do,")
        //.PatternMatch(MtgToken.That, "that")
        //.PatternMatch(MtgToken.And, "and")
        //.PatternMatch(MtgToken.If, "if")
        //.PatternMatch(MtgToken.Possessive, "'s", wrapInWordboundary: false)
        //.PatternMatch(MtgToken.Comma, ",", wrapInWordboundary: false)
        //.PatternMatch(MtgToken.Text, @"\S+", wrapInWordboundary: false)
        //.Build();
    }

    public static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, Type tokenCaptureType)
    {
        if (AppliedOrderTypes.Contains(tokenCaptureType))
            return tokenizerBuilder;

        var renderedRegex = GetRenderedRegex(tokenCaptureType);
        tokenizerBuilder.Match(Span.Regex(_renderedRegexes[tokenCaptureType]), tokenCaptureType);

        AppliedOrderTypes.Add(tokenCaptureType);

        return tokenizerBuilder;
    }

    public static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, string regexPattern, bool wrapInWordboundary = true)
    {
        tokenizerBuilder.Match(Span.Regex(regexPattern), typeof(string));
        return tokenizerBuilder;
    }
}

