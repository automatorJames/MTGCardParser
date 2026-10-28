namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illusionary Mask, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illusionary Mask</exampledoc>
[TokenizationOrder(-1)]
public class IllusionaryMaskRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, you may cast that card face down as a 2/2 creature spell without paying its mana cost"];
}
