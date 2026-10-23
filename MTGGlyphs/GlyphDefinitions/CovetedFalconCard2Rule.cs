namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Coveted Falcon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coveted Falcon</exampledoc>
[TokenizationOrder(-1)]
public class CovetedFalconCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card for each one they gained control of this way"];
}
