namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pyramids, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pyramids</exampledoc>
[TokenizationOrder(-1)]
public class PyramidsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• the next time target land would be destroyed this turn, remove all damage marked on it instead"];
}
