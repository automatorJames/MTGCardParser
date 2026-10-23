namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tannuk, Steadfast Second, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tannuk, Steadfast Second</exampledoc>
[Dependent]
public class TannukSteadfastSecond2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a card from your hand for its warp cost"];
}
