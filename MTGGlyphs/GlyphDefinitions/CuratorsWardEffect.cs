namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Curator's Ward, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curator's Ward</exampledoc>
[Dependent]
public class CuratorsWardEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it was historic, draw two cards"];
}
