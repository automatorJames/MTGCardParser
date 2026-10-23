namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Three Wishes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Three Wishes</exampledoc>
[Dependent]
public class ThreeWishesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put any of those cards you didn't play into your graveyard"];
}
