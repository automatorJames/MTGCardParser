namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Judith, Carnage Connoisseur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Judith, Carnage Connoisseur</exampledoc>
[TokenizationOrder(-1)]
public class JudithCarnageConnoisseurRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• that spell gains deathtouch and lifelink"];
}
