namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Furious Rise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Furious Rise</exampledoc>
[TokenizationOrder(-1)]
public class FuriousRiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play that card until you exile another card with this enchantment"];
}
