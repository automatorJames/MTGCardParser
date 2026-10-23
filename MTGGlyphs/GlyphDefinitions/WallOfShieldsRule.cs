namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wall of Shields, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Shields</exampledoc>
/// <examplecapture>banding (if any creatures with banding you control are blocking a creature, you divide that creature's combat damage, not its controller, among any of the creatures it's being blocked by</examplecapture>
[TokenizationOrder(-1)]
public class WallOfShieldsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["banding (if any creatures with banding you control are blocking a creature, you divide that creature's combat damage, not its controller, among any of the creatures it's being blocked by"];
}
