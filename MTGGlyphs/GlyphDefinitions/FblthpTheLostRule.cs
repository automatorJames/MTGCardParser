namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fblthp, the Lost, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fblthp, the Lost</exampledoc>
[TokenizationOrder(-1)]
public class FblthpTheLostRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it entered from your library or was cast from your library, draw two cards instead"];
}
