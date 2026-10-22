namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One named option in a choice: a Tarkir faction, a color, a land type, a card type or a creature type.</summary>
/// <exampledoc>Mangara's Equity</exampledoc>
/// <examplecapture>black</examplecapture>
[Dependent]
public class ChoiceOption : GlyphOneOf
{
    public TarkirFaction? Faction { get; set; }
    public ManaColor? Color { get; set; }
    public LandType? LandType { get; set; }
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
}
