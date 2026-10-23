namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wall of Caltrops, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Caltrops</exampledoc>
/// <examplecapture>if any creatures with banding you control are blocking a creature, you divide that creature's combat damage, not its controller, among any of the creatures it's being blocked by</examplecapture>
[Dependent]
public class WallOfCaltrops2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any creatures with banding you control are blocking a creature, you divide that creature's combat damage, not its controller, among any of the creatures it's being blocked by"];
}
