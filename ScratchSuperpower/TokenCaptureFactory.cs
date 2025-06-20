using MTGCardParser.TokenCaptures;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;
using System.ComponentModel.DataAnnotations;

namespace MTGCardParser;

public static class TokenCaptureFactory
{
    static readonly Tokenizer<MtgToken> _tokenizer;
    static readonly TokenCaptureRegistry _registry;
    static readonly Dictionary<MtgToken, string> _patterns = new();
    static readonly Dictionary<Type, MtgToken> _typesToTokens = new();
    static readonly Dictionary<MtgToken, Type> _tokensToTypes = new();

    static TokenCaptureFactory()
    {
        ScanForTokenCaptures();
        _registry = GetRegistry();
        _tokenizer = GetTokenizer();
    }

    public static TokenList<MtgToken> CleanAndTokenize(string cardText)
    {
        var cleaned = Regex.Replace(cardText, @"\([^)]*\)", "", RegexOptions.Singleline); // remove parens

        return _tokenizer.Tokenize(cleaned);
    }

    public static TokenCaptureRegistry GetRegistry() => new TokenCaptureRegistry(_patterns);

    static void ScanForTokenCaptures()
    {
        var tokenCaptureTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                typeof(ITokenCapture).IsAssignableFrom(t));

        var tokenValues = Enum.GetValues<MtgToken>().ToList();

        foreach (var tokenCaptureType in tokenCaptureTypes)
        {
            MtgToken? token = tokenValues
                .Where(x => x.ToString().Equals(tokenCaptureType.Name, StringComparison.OrdinalIgnoreCase))
                .Cast<MtgToken?>()
                .FirstOrDefault();

            if (token is null)
                throw new Exception($"No matching MtgToken exists for TokenCaptureType {tokenCaptureType.Name}");

            _typesToTokens[tokenCaptureType] = token.Value;
            _tokensToTypes[token.Value] = tokenCaptureType;
            var regexTemplateProperty = tokenCaptureType.GetProperty(nameof(ITokenCapture.RegexTemplate), BindingFlags.Static | BindingFlags.Public);
            var regexTemplate = regexTemplateProperty?.GetValue(null) as string;
            var regexPattern = regexTemplate.GetRegex(tokenCaptureType);
            _patterns[token.Value] = regexPattern;
        }
    }

    public static Tokenizer<MtgToken> GetTokenizer()
    {
        // Three basic kinds of tokens:
        //      1) Flow control (not many of these: newline, period, etc.)
        //      2) Boolean (the token either exists or it doesn't, like "{this}"
        //      3) Variable (there's an ITokenCapture implementation class w/ properties)

        var tokenizerBuilder = new TokenizerBuilder<MtgToken>();
        var tokenizerBuilderz = new TokenizerBuilder<Type>();

        tokenizerBuilder
            .Ignore(Span.Regex(@"[ \t]+"))
            .PatternMatch(MtgToken.Newline, "\n", wrapInWordboundary: false)
            .PatternMatch(MtgToken.Period, ".", wrapInWordboundary: false);


        tokenizerBuilderz
            .Ignore(Span.Regex(@"[ \t]+"))
            .MatchRoofedCottages(typeof(AtOrUntilPlayerPhase))
            .MatchRoofedCottages(@"\S+", wrapInWordboundary: false);

        var thingy = tokenizerBuilderz.Build();
        var tokes = thingy.Tokenize("at the beginning of your upkeep, sacrifice {this} unless you pay {w}{w}.").ToList();
        var toke = tokes.First();
        var instance = HydrateFromToken(toke);

        foreach (var item in _registry)
            tokenizerBuilder.Match(Span.Regex(item.Value), item.Key);

        tokenizerBuilder.PatternMatch(MtgToken.Text, @"\S+", wrapInWordboundary: false);

        return tokenizerBuilder.Build();


        return new TokenizerBuilder<MtgToken>()
        .Ignore(Span.Regex(@"[ \t]+"))
        //.PatternMatch(MtgToken.Newline, "\n", wrapInWordboundary: false)
        //.PatternMatch(MtgToken.Period, ".", wrapInWordboundary: false)
        .PatternMatch(MtgToken.This, @"\{this\}", wrapInWordboundary: false)
        .PatternMatch(MtgToken.Target, "target(s|ed)?")
        .PatternMatch(MtgToken.Until, "until")
        .PatternMatch(MtgToken.Tap, @"\{t\}", wrapInWordboundary: false)
        .PatternMatch(MtgToken.X, "x")
        .PatternMatch(MtgToken.Destroy, "destroy(s|ed)?")
        .PatternMatch(MtgToken.ThatDamage, "that damage")
        //.PatternMatch(MtgToken.PayCost)
        //.PatternMatch(MtgToken.Color)
        //.PatternMatch(MtgToken.EnchantCardType)
        //.PatternMatch(MtgToken.CardType)
        //.PatternMatch(MtgToken.SubType)
        //.PatternMatch(MtgToken.CardQuantityChange)
        //.PatternMatch(MtgToken.Keyword)
        //.PatternMatch(MtgToken.PlusMinusPowerToughness)
        //.PatternMatch(MtgToken.AddMana)
        //.PatternMatch(MtgToken.ManaCost)
        //.PatternMatch(MtgToken.GamePhase)
        //.PatternMatch(MtgToken.Who)
        //.PatternMatch(MtgToken.DealDamageAmount)
        //.PatternMatch(MtgToken.NextDamageAmount)
        //.PatternMatch(MtgToken.Quantity)
        .PatternMatch(MtgToken.When, "when(ever)?")
        .PatternMatch(MtgToken.Has, "(has|had|have)")
        .PatternMatch(MtgToken.Get, "get(s)?")
        .PatternMatch(MtgToken.From, "from")
        .PatternMatch(MtgToken.May, "may")
        .PatternMatch(MtgToken.DamageToAny, "damage to any")
        .PatternMatch(MtgToken.IfYouDo, "if you do,")
        .PatternMatch(MtgToken.That, "that")
        .PatternMatch(MtgToken.And, "and")
        .PatternMatch(MtgToken.If, "if")
        .PatternMatch(MtgToken.Possessive, "'s", wrapInWordboundary: false)
        .PatternMatch(MtgToken.Comma, ",", wrapInWordboundary: false)
        .PatternMatch(MtgToken.Text, @"\S+", wrapInWordboundary: false)
        .Build();
    }

    //public static ITokenCapture HydrateFromToken(Token<MtgToken> token)
    //{
    //    if (!_tokensToTypes.ContainsKey(token.Kind))
    //        throw new Exception($"No matching ITokenCapture type registered for token {token}");
    //
    //    var instanceType = _tokensToTypes[token.Kind];
    //    var instance = (ITokenCapture)Activator.CreateInstance(instanceType);
    //    instance.PopulateScalarValues(token.Span.Source);
    //
    //    return instance;
    //}

    public static ITokenCapture HydrateFromToken(Token<Type> token)
    {
        var instance = (ITokenCapture)Activator.CreateInstance(token.Kind);
        instance.PopulateScalarValues(token.Span.Source);

        return instance;
    }


    public static TokenizerBuilder<MtgToken> PatternMatch(this TokenizerBuilder<MtgToken> tokenizerBuilder, MtgToken token)
    {
        var pattern = _registry[token];

        if (pattern is null)
            throw new Exception($"No regex defined for token {token}");

        tokenizerBuilder.Match(Span.Regex(pattern), token);

        return tokenizerBuilder;
    }

    public static TokenizerBuilder<MtgToken> PatternMatch(this TokenizerBuilder<MtgToken> tokenizerBuilder, MtgToken token, string pattern, bool wrapInWordboundary = true)
    {
        if (wrapInWordboundary)
            pattern = $@"\b{pattern}\b";

        if (pattern.Length == 1)
            tokenizerBuilder.Match(Character.EqualTo(pattern.First()), token);
        else
            tokenizerBuilder.Match(Span.Regex(pattern), token);

        return tokenizerBuilder;
    }

    public static TokenizerBuilder<Type> MatchRoofedCottages(this TokenizerBuilder<Type> tokenizerBuilder, Type tokenCaptureType, bool wrapInWordboundary = true)
    {
        var regexTemplateProperty = tokenCaptureType.GetProperty(nameof(ITokenCapture.RegexTemplate), BindingFlags.Static | BindingFlags.Public);
        var regexTemplate = regexTemplateProperty?.GetValue(null) as string;
        var regexPattern = regexTemplate.GetRegex(tokenCaptureType);

        tokenizerBuilder.Match(Span.Regex(regexPattern), tokenCaptureType);

        return tokenizerBuilder;
    }

    public static TokenizerBuilder<Type> MatchRoofedCottages(this TokenizerBuilder<Type> tokenizerBuilder, string pattern, bool wrapInWordboundary = true)
    {
        if (wrapInWordboundary)
            pattern = $@"\b{pattern}\b";

        tokenizerBuilder.Match(Span.Regex(pattern), typeof(string));
        return tokenizerBuilder;
    }


}

