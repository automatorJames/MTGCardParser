namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of North Star, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>North Star</exampledoc>
[Dependent]
public class NorthStarCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["additional costs are still paid normally"];
}
