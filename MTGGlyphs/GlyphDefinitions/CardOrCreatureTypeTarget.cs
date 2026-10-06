namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a targeted permanent is: a card type ("land") or a creature type ("wall").</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>land</examplecapture>
[Dependent]
public class CardOrCreatureTypeTarget : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
}