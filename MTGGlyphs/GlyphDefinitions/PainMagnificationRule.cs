namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pain Magnification, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pain Magnification</exampledoc>
[TokenizationOrder(-1)]
public class PainMagnificationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent is dealt 3 or more damage by a single source, that player discards a card"];
}
