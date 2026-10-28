namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Soulgorger Orgg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soulgorger Orgg</exampledoc>
[Dependent]
public class SoulgorgerOrggEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to the life you lost when it entered"];
}
