namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Discerning Financier, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Discerning Financier</exampledoc>
[Dependent]
public class DiscerningFinancierEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose another player"];
}
