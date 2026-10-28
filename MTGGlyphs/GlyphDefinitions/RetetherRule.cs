namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Retether, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Retether</exampledoc>
[TokenizationOrder(-1)]
public class RetetherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only creatures can be enchanted this way"];
}
