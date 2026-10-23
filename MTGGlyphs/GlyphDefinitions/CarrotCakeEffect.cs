namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Carrot Cake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Carrot Cake</exampledoc>
[Dependent]
public class CarrotCakeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 white rabbit creature token and scry 1"];
}
