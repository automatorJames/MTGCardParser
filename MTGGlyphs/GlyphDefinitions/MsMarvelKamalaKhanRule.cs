namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ms. Marvel, Kamala Khan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ms. Marvel, Kamala Khan</exampledoc>
[TokenizationOrder(-1)]
public class MsMarvelKamalaKhanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["marvel's base power is equal to the number of cards in your hand"];
}
