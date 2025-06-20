/*using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Linq;
using System.Collections.Generic;
using MTGCardParser;

namespace MTGCardParser;

// Type aliases for cleaner parser definitions
using MtgTokenListParser = TokenListParser<MtgToken, IAbility>;
using CostParser = TokenListParser<MtgToken, ICost>;
using EffectParser = TokenListParser<MtgToken, IEffect>;

public static class MtgParser
{
    // --- Basic Building Blocks ---
    private static TokenListParser<MtgToken, string> Word(string text) =>
        Token.EqualToValue(MtgToken.Text, text).Select(t => t.ToStringValue());

    // --- Cost Parsers ---
    private static CostParser TapCostParser { get; } =
        from l in Token.EqualTo(MtgToken.LeftBrace)
        from t in Word("T")
        from r in Token.EqualTo(MtgToken.RightBrace)
        select (ICost)new TapCost();

    private static TokenListParser<MtgToken, ManaCost> ManaSymbolParser { get; } =
        Token.EqualTo(MtgToken.Number).Select(n => new ManaCost(Generic: int.Parse(n.ToStringValue())))
            .Or(Word("W").Select(_ => new ManaCost(W: 1)))
            .Or(Word("U").Select(_ => new ManaCost(U: 1)))
            .Or(Word("B").Select(_ => new ManaCost(B: 1)))
            .Or(Word("R").Select(_ => new ManaCost(R: 1)))
            .Or(Word("G").Select(_ => new ManaCost(G: 1)));

    private static CostParser ManaCostParser { get; } =
        from l in Token.EqualTo(MtgToken.LeftBrace)
        from symbols in ManaSymbolParser.Many()
        from r in Token.EqualTo(MtgToken.RightBrace)
        select (ICost)symbols.Aggregate(new ManaCost(), (acc, cost) =>
            new ManaCost(
                acc.Generic + cost.Generic, acc.W + cost.W, acc.U + cost.U,
                acc.B + cost.B, acc.R + cost.R, acc.G + cost.G
            ));

    private static CostParser Cost { get; } =
        TapCostParser.Try().Or(ManaCostParser);

    private static TokenListParser<MtgToken, List<ICost>> CostList { get; } =
        Cost.ManyDelimitedBy(Token.EqualTo(MtgToken.Comma))
            .Select(costs => costs.ToList());

    // --- Effect Parsers ---

    // FIX: The AddManaEffect parser now handles a sequence of mana costs like {U}{R}.
    private static EffectParser AddManaEffect { get; } =
        from add in Word("Add")
            // 1. Parse ONE OR MORE mana costs in a row. ManaCostParser handles one {..} blob.
        from costs in ManaCostParser.Many()
            // 2. Aggregate the list of ManaCost objects into a single one.
        select (IEffect)new AddManaEffect(
            costs.Cast<ManaCost>().Aggregate(new ManaCost(), (acc, cost) =>
                new ManaCost(
                    acc.Generic + cost.Generic, acc.W + cost.W, acc.U + cost.U,
                    acc.B + cost.B, acc.R + cost.R, acc.G + cost.G
                )
            )
        );

    private static EffectParser GainLifeEffect { get; } =
        from you in Word("you").OptionalOrDefault()
        from gain in Word("gain")
        from amount in Token.EqualTo(MtgToken.Number).Apply(Numerics.IntegerInt32)
        from life in Word("life")
        select (IEffect)new GainLifeEffect(amount);

    private static EffectParser DealDamageEffect { get; } =
        from deals in Word("deals").Or(Word("deal"))
        from amount in Token.EqualTo(MtgToken.Number).Apply(Numerics.IntegerInt32)
        from damage in Word("damage")
        from to in Word("to")
        from target in Word("any").Then(t => Word("target"))
        select (IEffect)new DealDamageEffect(amount, new AnyTarget());

    private static EffectParser Effect { get; } =
        AddManaEffect.Try().Or(GainLifeEffect.Try()).Or(DealDamageEffect);

    private static EffectParser OptionalEffect { get; } =
        from you in Word("you").OptionalOrDefault()
        from may in Word("may")
        from effect in Effect
        select (IEffect)new OptionalEffect(effect);



    private static EffectParser AnyEffect { get; } =
        OptionalEffect.Try().Or(Effect);



    private static TokenListParser<MtgToken, List<IEffect>> EffectList { get; } =
        AnyEffect.Select(e => new List<IEffect> { e });

    // --- Full Ability Parsers ---

    private static MtgTokenListParser ActivatedAbilityParser { get; } =
        from costs in CostList
        from colon in Token.EqualTo(MtgToken.Colon)
        from effects in EffectList
        select (IAbility)new ActivatedAbility(costs, effects);

    private static MtgTokenListParser StaticAbilityParser { get; } =
        Word("Flying").Or(Word("Vigilance")).Or(Word("Trample"))
        .Select(kw => (IAbility)new StaticAbility(kw));

    private static TokenListParser<MtgToken, string> TriggerConditionParser { get; } =
        from tokens in Token.Matching<MtgToken>(kind => kind != MtgToken.Comma, "any token except comma").Many()
        where tokens.Any()
        select tokens.First().Span.Until(tokens.Last().Span).ToStringValue();

    private static MtgTokenListParser TriggeredAbilityParser { get; } =
        from triggerText in TriggerConditionParser
        from comma in Token.EqualTo(MtgToken.Comma)
        from effects in EffectList
        select (IAbility)new TriggeredAbility(
            triggerText.Trim(),
            effects);

    // --- The Master Parser ---
    public static MtgTokenListParser Ability { get; } =
        ActivatedAbilityParser.Try()
        .Or(TriggeredAbilityParser.Try())
        .Or(StaticAbilityParser)
        .AtEnd();
}*/