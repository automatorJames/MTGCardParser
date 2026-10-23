namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sizzling Soloist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sizzling Soloist</exampledoc>
[TokenizationOrder(-1)]
public class SizzlingSoloistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this is the second time this ability has resolved this turn, that creature attacks during its controller's next combat phase if able"];
}
