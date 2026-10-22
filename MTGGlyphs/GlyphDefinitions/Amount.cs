namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount defined by the game state: a count ("the number of swamps you control", "the number of opponents you have"), a characteristic ("its power", "the power of the exiled card"), what an effect just did ("the life lost this way"), devotion ("your devotion to black"), the greatest or total of a characteristic in a group, a life total, or "the difference" a preceding comparison set up.</summary>
/// <exampledoc>Karma</exampledoc>
/// <examplecapture>the number of swamps they control</examplecapture>
[Dependent]
public class Amount : GlyphOneOf
{
    public OpponentCount OpponentCount { get; set; }
    public NumberOf NumberOf { get; set; }
    public CharacteristicOf CharacteristicOf { get; set; }
    public CharacteristicOfObject CharacteristicOfObject { get; set; }
    public AmountThisWay ThisWay { get; set; }
    public DevotionTo Devotion { get; set; }
    public GreatestAmong Greatest { get; set; }
    public TotalOf Total { get; set; }
    public LifeTotalOf LifeTotal { get; set; }
    [RegexPattern("the difference")]
    public bool Difference { get; set; }
}
