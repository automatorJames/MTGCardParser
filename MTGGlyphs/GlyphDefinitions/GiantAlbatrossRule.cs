namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Giant Albatross, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Giant Albatross</exampledoc>
[TokenizationOrder(-1)]
public class GiantAlbatrossRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, for each creature that dealt damage to this creature this turn, destroy that creature unless its controller pays 2 life"];
}
