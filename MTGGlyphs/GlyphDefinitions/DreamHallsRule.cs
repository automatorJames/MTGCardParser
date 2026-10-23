namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dream Halls, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dream Halls</exampledoc>
[TokenizationOrder(-1)]
public class DreamHallsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["rather than pay the mana cost for a spell, its controller may discard a card that shares a color with that spell"];
}
