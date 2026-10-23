namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Coax from the Blind Eternities, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coax from the Blind Eternities</exampledoc>
[TokenizationOrder(-1)]
public class CoaxFromTheBlindEternitiesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may reveal an eldrazi card you own from outside the game or choose a face-up eldrazi card you own in exile"];
}
