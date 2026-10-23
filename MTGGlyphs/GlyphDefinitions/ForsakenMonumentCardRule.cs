namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Forsaken Monument, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Forsaken Monument</exampledoc>
[TokenizationOrder(-1)]
public class ForsakenMonumentCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you tap a permanent for {c}, add an additional {c}"];
}
