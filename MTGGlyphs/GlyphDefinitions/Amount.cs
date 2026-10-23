namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount defined by the game state: a count ("the number of swamps you control", "the number of opponents you have"), how many times something happened to an object ("the number of times that spell was kicked"), a characteristic ("its power", "the power of the exiled card"), what an effect just did ("the life lost this way"), devotion ("your devotion to black"), the greatest or total of a characteristic in a group, the largest group sharing a creature type, a life total, the mana spent on a spell ("the amount of mana spent to cast that spell"), the life gained or lost this turn ("the amount of life you gained this turn", "the total life lost by your opponents this turn"), the damage dealt this turn ("the damage dealt to your opponents this turn"), or "the difference" a preceding comparison set up.</summary>
/// <exampledoc>Karma</exampledoc>
/// <examplecapture>the number of swamps they control</examplecapture>
[Dependent]
public class Amount : GlyphOneOf
{
    public OpponentCount OpponentCount { get; set; }
    public NumberOf NumberOf { get; set; }
    public NumberOfTimes NumberOfTimes { get; set; }
    public CharacteristicOf CharacteristicOf { get; set; }
    public CharacteristicOfObject CharacteristicOfObject { get; set; }
    public AmountThisWay ThisWay { get; set; }
    public DevotionTo Devotion { get; set; }
    public GreatestAmong Greatest { get; set; }
    public LargestSharedTypeCount LargestSharedType { get; set; }
    public TotalOf Total { get; set; }
    public LifeTotalOf LifeTotal { get; set; }
    public ManaSpentAmount ManaSpent { get; set; }
    public LifeChangedAmount LifeChanged { get; set; }
    public TotalLifeLostBy TotalLifeLost { get; set; }
    public DamageDealtThisTurn DamageDealt { get; set; }
    [RegexPattern("the difference")]
    public bool Difference { get; set; }
}
