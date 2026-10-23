namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Doom Foretold, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doom Foretold</exampledoc>
/// <examplecapture>if that player can't, they discard a card, they lose 2 life, you draw a card, you gain 2 life, you create a 2/2 white knight creature token with vigilance, then you sacrifice this enchantment</examplecapture>
[TokenizationOrder(-1)]
public class DoomForetoldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that player can't, they discard a card, they lose 2 life, you draw a card, you gain 2 life, you create a 2/2 white knight creature token with vigilance, then you sacrifice this enchantment"];
}
