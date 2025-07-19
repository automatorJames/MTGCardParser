namespace MTGPlexer.Static;

public static partial class TokenTypeRegistry
{
    static List<Type> TypeOrderList =
    [
        typeof(AtOrUntilPlayerPhase),
        typeof(This),
        typeof(ActivatedAbility),
        typeof(OptionalPayCost),
        typeof(GainOrLoseAbility),
        typeof(EnchantCard),
        typeof(CardKeyword),
        typeof(AtOrUntilPlayerPhase),
        typeof(IfYouDo),
        typeof(EnchantedCard),
        typeof(LifeChangeQuantity),
        typeof(Parenthetical),
        typeof(ManaValue),
        typeof(PunctuationTerminal),
        typeof(PunctuationEnclosing)
    ];
}

