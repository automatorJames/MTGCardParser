namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rhuk, Hexgold Nabber, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rhuk, Hexgold Nabber</exampledoc>
[TokenizationOrder(-1)]
public class RhukHexgoldNabberCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an equipped creature you control other than rhuk attacks or dies, you may attach all equipment attached to that creature to rhuk"];
}
