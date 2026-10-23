namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kami's Flare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kami's Flare</exampledoc>
[TokenizationOrder(-1)]
public class KamisFlareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "also deals 2 damage to that permanent's controller if you control a modified creature"];
}
