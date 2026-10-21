namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount defined by the game state: a count ("the number of swamps you control"), a characteristic ("its power", "the power of the exiled card"), what an effect just did ("the life lost this way"), devotion ("your devotion to black"), the greatest or total of a characteristic in a group, or a life total.</summary>
/// <exampledoc>Karma</exampledoc>
/// <examplecapture>the number of swamps they control</examplecapture>
[Dependent]
public class Amount : GlyphOneOf
{
    public NumberOf NumberOf { get; set; }
    public CharacteristicOf CharacteristicOf { get; set; }
    public CharacteristicOfObject CharacteristicOfObject { get; set; }
    public AmountThisWay ThisWay { get; set; }
    public DevotionTo Devotion { get; set; }
    public GreatestAmong Greatest { get; set; }
    public TotalOf Total { get; set; }
    public LifeTotalOf LifeTotal { get; set; }
}
