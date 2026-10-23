namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mogg Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mogg Assassin</exampledoc>
[TokenizationOrder(-1)]
public class MoggAssassin2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win the flip, destroy the creature you chose"];
}
