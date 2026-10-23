namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jiang Yanggu, Alone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jiang Yanggu, Alone</exampledoc>
[Dependent]
public class JiangYangguAloneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard a card, then draw a card"];
}
