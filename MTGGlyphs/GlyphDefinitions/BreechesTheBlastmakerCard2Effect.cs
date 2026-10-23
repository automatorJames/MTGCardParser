namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Breeches, the Blastmaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Breeches, the Blastmaker</exampledoc>
[Dependent]
public class BreechesTheBlastmakerCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy that spell"];
}
