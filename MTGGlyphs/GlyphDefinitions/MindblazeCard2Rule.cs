namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mindblaze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindblaze</exampledoc>
[TokenizationOrder(-1)]
public class MindblazeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals their library"];
}
