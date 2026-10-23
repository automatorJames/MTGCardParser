namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bound // Determined, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bound // Determined</exampledoc>
[TokenizationOrder(-1)]
public class BoundRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return up to x cards from your graveyard to your hand, where x is the number of colors that creature was"];
}
