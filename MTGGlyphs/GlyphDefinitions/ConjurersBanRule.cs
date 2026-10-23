namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Conjurer's Ban, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Conjurer's Ban</exampledoc>
[TokenizationOrder(-1)]
public class ConjurersBanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, spells with the chosen name can't be cast and lands with the chosen name can't be played"];
}
