namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cruel Calculations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cruel Calculations</exampledoc>
[TokenizationOrder(-1)]
public class CruelCalculationsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw x cards, where x is the number of cards that were put into target player's graveyard from their library this turn"];
}
