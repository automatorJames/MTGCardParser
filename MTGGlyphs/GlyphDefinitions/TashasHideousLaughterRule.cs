namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tasha's Hideous Laughter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tasha's Hideous Laughter</exampledoc>
[TokenizationOrder(-1)]
public class TashasHideousLaughterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent exiles cards from the top of their library until that player has exiled cards with total mana value 20 or greater"];
}
