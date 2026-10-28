namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Breeches, the Blastmaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Breeches, the Blastmaker</exampledoc>
[TokenizationOrder(-1)]
public class BreechesTheBlastmakerRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you lose the flip, breeches deals damage equal to that spell's mana value to any target"];
}
