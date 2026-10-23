namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lens of Clarity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lens of Clarity</exampledoc>
[TokenizationOrder(-1)]
public class LensOfClarityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may look at the top card of your library and at face-down creatures you don't control any time"];
}
