namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Carrot Cake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Carrot Cake</exampledoc>
[TokenizationOrder(-1)]
public class CarrotCakeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this artifact enters and when you sacrifice it, create a 1/1 white rabbit creature token and scry 1"];
}
