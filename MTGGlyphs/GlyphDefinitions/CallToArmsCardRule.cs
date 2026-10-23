namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Call to Arms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Call to Arms</exampledoc>
[TokenizationOrder(-1)]
public class CallToArmsCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when the chosen color isn't the most common color among nontoken permanents the chosen player controls or is tied for most common, sacrifice this enchantment"];
}
