namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ral Zarek, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ral Zarek</exampledoc>
[Dependent]
public class RalZarekCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip five coins"];
}
