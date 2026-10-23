namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rhuk, Hexgold Nabber, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rhuk, Hexgold Nabber</exampledoc>
[Dependent]
public class RhukHexgoldNabberEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may attach all equipment attached to that creature to rhuk"];
}
