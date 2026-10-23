namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Masked Gorgon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Masked Gorgon</exampledoc>
[TokenizationOrder(-1)]
public class MaskedGorgonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["green creatures and white creatures have protection from gorgons"];
}
