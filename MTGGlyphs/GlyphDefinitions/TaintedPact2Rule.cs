namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tainted Pact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tainted Pact</exampledoc>
[TokenizationOrder(-1)]
public class TaintedPact2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put that card into your hand unless it has the same name as another card exiled this way"];
}
