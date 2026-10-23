namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Portent of Calamity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Portent of Calamity</exampledoc>
[TokenizationOrder(-1)]
public class PortentOfCalamityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell from among the exiled cards without paying its mana cost if you exiled four or more cards this way"];
}
