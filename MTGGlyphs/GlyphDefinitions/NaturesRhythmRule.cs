namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nature's Rhythm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nature's Rhythm</exampledoc>
[TokenizationOrder(-1)]
public class NaturesRhythmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may tap a creature you control to reduce that cost by an amount of generic mana equal to its power"];
}
