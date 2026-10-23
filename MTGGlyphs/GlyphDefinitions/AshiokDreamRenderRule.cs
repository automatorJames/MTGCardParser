namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ashiok, Dream Render, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashiok, Dream Render</exampledoc>
[TokenizationOrder(-1)]
public class AshiokDreamRenderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells and abilities your opponents control can't cause their controller to search their library"];
}
