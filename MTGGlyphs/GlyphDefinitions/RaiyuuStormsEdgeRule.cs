namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Raiyuu, Storm's Edge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Raiyuu, Storm's Edge</exampledoc>
[TokenizationOrder(-1)]
public class RaiyuuStormsEdgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first combat phase of the turn, there is an additional combat phase after this phase"];
}
