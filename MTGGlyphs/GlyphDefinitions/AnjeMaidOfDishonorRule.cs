namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Anje, Maid of Dishonor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anje, Maid of Dishonor</exampledoc>
[TokenizationOrder(-1)]
public class AnjeMaidOfDishonorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever anje and/or one or more other vampires you control enter, create a blood token"];
}
