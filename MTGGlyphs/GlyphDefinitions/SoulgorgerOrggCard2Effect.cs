namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Soulgorger Orgg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soulgorger Orgg</exampledoc>
[Dependent]
public class SoulgorgerOrggCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you lose all but 1 life"];
}
