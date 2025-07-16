namespace MTGPlexer.Static;

public static partial class TokenTypeRegistry
{
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
            .Match(typeof(Parenthetical))
            .Match(typeof(ManaValue))
            .Match(typeof(PunctuationTerminal))
            .Match(typeof(PunctuationEnclosing));

        var remainingLengthOrderedTypeRegexItems = Templates
            .Where(x => !AppliedOrderTypes.Contains(x.Key))
            .OrderByDescending(x => x.Value.RenderedRegexString.Length)
            .ToList();

        // Apply assembly types that weren't applied above (failsafe for laziness)
        foreach (var item in remainingLengthOrderedTypeRegexItems)
            tokenizerBuilder.Match(item.Key);

        // Catch anything else with the default string pattern
        tokenizerBuilder.Match(typeof(DefaultUnmatchedString));

        Tokenizer = tokenizerBuilder.Build();
    }

    static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, Type tokenCaptureType)
    {
        if (AppliedOrderTypes.Contains(tokenCaptureType) || _invalidTypes.Contains(tokenCaptureType))
            return tokenizerBuilder;

        tokenizerBuilder.Match(Span.Regex(Templates[tokenCaptureType].RenderedRegexString), tokenCaptureType);
        AppliedOrderTypes.Add(tokenCaptureType);

        return tokenizerBuilder;
    }
}

