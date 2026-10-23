namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ajani Unrelenting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ajani Unrelenting</exampledoc>
[Dependent]
public class AjaniUnrelentingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ajani deals 4 damage to each creature except for tokens you control"];
}
