namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gideon's Triumph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon's Triumph</exampledoc>
[TokenizationOrder(-1)]
public class GideonsTriumph2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent sacrifices a creature of their choice that attacked or blocked this turn"];
}
