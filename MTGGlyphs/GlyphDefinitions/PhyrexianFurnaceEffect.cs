namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Phyrexian Furnace, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Furnace</exampledoc>
[Dependent]
public class PhyrexianFurnaceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the bottom card of target player's graveyard"];
}
