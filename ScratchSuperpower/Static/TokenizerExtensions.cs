using Superpower.Parsers;
using Superpower.Tokenizers;

namespace MTGCardParser.Static;

//public static class TokenizerExtensions
//{
//    //static readonly RegexPatternGetter regexPatternGetter = new();
//    static readonly TokenCaptureRegistry _registry;
//
//    static TokenizerExtensions()
//    {
//        _registry = TokenCaptureFactory.GetRegistry();
//    }
//
//    public static TokenizerBuilder<MtgToken> PatternMatch(this TokenizerBuilder<MtgToken> tokenizerBuilder, MtgToken token)
//    {
//        var pattern = _registry[token];
//
//        if (pattern is null)
//            throw new Exception($"No regex defined for token {token}");
//
//        tokenizerBuilder.Match(Span.Regex(pattern), token);
//
//        return tokenizerBuilder;
//    }
//
//    public static TokenizerBuilder<MtgToken> PatternMatch(this TokenizerBuilder<MtgToken> tokenizerBuilder, MtgToken token, string pattern, bool wrapInWordboundary = true)
//    {
//        if (wrapInWordboundary)
//            pattern = $@"\b{pattern}\b";
//
//        if (pattern.Length == 1)
//            tokenizerBuilder.Match(Character.EqualTo(pattern.First()), token);
//        else
//            tokenizerBuilder.Match(Span.Regex(pattern), token);
//
//        return tokenizerBuilder;
//    }
//
//}

