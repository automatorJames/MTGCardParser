using MTGCardParser.Static;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;
using System;
using System.Text.RegularExpressions;

namespace MTGCardParser;

// The categories for our tokens
public enum MtgToken
{
    Newline,
    Period,
    CardType,
    SubType,
    ManaCost,
    Tap,
    X,
    Color,
    This,
    Target,
    LifeChange,
    CardQuantityChange,
    Keyword,
    Text,
    Number,
    Comma,
    Colon,
    LeftBrace,
    RightBrace,
    PlusMinusPowerToughness,
    Test
}

public static class MtgTokenizer
{
    // Build a tokenizer that recognizes our tokens
    public static Tokenizer<MtgToken> Create()
    {
        return new TokenizerBuilder<MtgToken>()
            .Ignore(Span.Regex(@"[ \t]+|\([^)]*\)"))
            .Match(Span.EqualTo("\n"), MtgToken.Newline)
            .Match(Span.EqualTo("."), MtgToken.Period)
            .Match(Span.EqualTo("{this}"), MtgToken.This)
            .Match(Span.Regex("target(s|ed)?"), MtgToken.Target)
            .Match(Span.EqualTo("{t}"), MtgToken.Tap)
            .Match(Span.EqualTo("x"), MtgToken.X)
            .Match(Span.Regex(RegexPatterns.Color), MtgToken.Color)
            .Match(Span.Regex(RegexPatterns.LifeChange), MtgToken.LifeChange)
            .Match(Span.EqualTo("x"), MtgToken.X)
            .Match(Span.Regex(RegexPatterns.CardType), MtgToken.CardType)
            .Match(Span.Regex(RegexPatterns.Subtype), MtgToken.SubType)
            .Match(Span.Regex(RegexPatterns.CardQuantityChange), MtgToken.CardQuantityChange)
            .Match(Span.Regex(RegexPatterns.Keyword), MtgToken.Keyword)
            .Match(Span.Regex(RegexPatterns.PlusMinusPowerToughness), MtgToken.PlusMinusPowerToughness)
            .Match(Span.Regex(RegexPatterns.ManaCost), MtgToken.ManaCost)
            .Match(Character.EqualTo('{'), MtgToken.LeftBrace)
            .Match(Character.EqualTo('}'), MtgToken.RightBrace)
            .Match(Character.EqualTo(':'), MtgToken.Colon)
            .Match(Character.EqualTo(','), MtgToken.Comma)
            .Match(Numerics.IntegerInt32, MtgToken.Number)
            .Match(Span.Regex(@"\S+"), MtgToken.Text)
            .Build();
    }
}