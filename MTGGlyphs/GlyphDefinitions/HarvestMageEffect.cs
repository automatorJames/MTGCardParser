namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Harvest Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Harvest Mage</exampledoc>
/// <examplecapture>until end of turn, if you tap a land for mana, it produces one mana of a color of your choice instead of any other type and amount</examplecapture>
[Dependent]
public class HarvestMageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, if you tap a land for mana, it produces one mana of a color of your choice instead of any other type and amount"];
}
