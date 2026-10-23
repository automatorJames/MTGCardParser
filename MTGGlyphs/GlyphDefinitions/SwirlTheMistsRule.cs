namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Swirl the Mists, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swirl the Mists</exampledoc>
[TokenizationOrder(-1)]
public class SwirlTheMistsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["all instances of color words in the text of spells and permanents are changed to the chosen color word"];
}
