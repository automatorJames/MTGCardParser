namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Breeches, the Blastmaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Breeches, the Blastmaker</exampledoc>
[Dependent]
public class BreechesTheBlastmakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["breeches deals damage equal to that spell's mana value to any target"];
}
