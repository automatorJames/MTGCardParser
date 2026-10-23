namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Trove of Temptation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trove of Temptation</exampledoc>
[TokenizationOrder(-1)]
public class TroveOfTemptationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent must attack you or a planeswalker you control with at least one creature each combat if able"];
}
