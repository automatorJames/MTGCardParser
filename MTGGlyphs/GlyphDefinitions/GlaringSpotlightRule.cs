namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Glaring Spotlight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glaring Spotlight</exampledoc>
[TokenizationOrder(-1)]
public class GlaringSpotlightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures your opponents control with hexproof can be the targets of spells and abilities you control as though they didn't have hexproof"];
}
