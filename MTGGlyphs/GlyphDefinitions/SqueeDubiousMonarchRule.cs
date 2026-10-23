namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Squee, Dubious Monarch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Squee, Dubious Monarch</exampledoc>
[TokenizationOrder(-1)]
public class SqueeDubiousMonarchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard by paying {3}{r} and exiling four other cards from your graveyard rather than paying its mana cost"];
}
