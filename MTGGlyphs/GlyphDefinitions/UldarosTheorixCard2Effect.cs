namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Uldaros Theorix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uldaros Theorix</exampledoc>
[Dependent]
public class UldarosTheorixCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["permanent spells cast this way become tokens"];
}
