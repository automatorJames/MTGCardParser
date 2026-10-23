namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mystic Subdual, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mystic Subdual</exampledoc>
[Dependent]
public class MysticSubdualEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mutating onto the creature won't give it new abilities"];
}
