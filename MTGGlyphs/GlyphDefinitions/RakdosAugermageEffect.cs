namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rakdos Augermage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos Augermage</exampledoc>
[Dependent]
public class RakdosAugermageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal your hand and discard a card of target opponent's choice"];
}
