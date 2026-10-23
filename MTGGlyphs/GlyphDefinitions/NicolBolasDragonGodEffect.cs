namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nicol Bolas, Dragon-God, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nicol Bolas, Dragon-God</exampledoc>
[Dependent]
public class NicolBolasDragonGodEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who doesn't control a legendary creature or planeswalker loses the game"];
}
