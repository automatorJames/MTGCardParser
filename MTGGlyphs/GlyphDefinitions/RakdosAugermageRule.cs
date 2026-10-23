namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rakdos Augermage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos Augermage</exampledoc>
[TokenizationOrder(-1)]
public class RakdosAugermageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player reveals their hand and discards a card of your choice"];
}
