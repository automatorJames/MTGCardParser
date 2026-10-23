namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spider-Man India, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Man India</exampledoc>
[TokenizationOrder(-1)]
public class SpiderManIndiaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["pavitr's sevā — whenever you cast a creature spell, put a +1/+1 counter on target creature you control"];
}
