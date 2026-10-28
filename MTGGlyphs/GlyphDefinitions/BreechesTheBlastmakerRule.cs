namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Breeches, the Blastmaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Breeches, the Blastmaker</exampledoc>
[TokenizationOrder(-1)]
public class BreechesTheBlastmakerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you win the flip, copy that spell"];
}
