namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Panglacial Wurm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Panglacial Wurm</exampledoc>
[TokenizationOrder(-1)]
public class PanglacialWurmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["while you're searching your library, you may cast this card from your library"];
}
