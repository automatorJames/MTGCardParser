namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phyrexian Unlife, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Unlife</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianUnlifeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you have 0 or less life, all damage is dealt to you as though its source had infect"];
}
