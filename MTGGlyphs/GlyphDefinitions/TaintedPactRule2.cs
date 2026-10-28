namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tainted Pact, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tainted Pact</exampledoc>
[TokenizationOrder(-1)]
public class TaintedPactRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until you put a card into your hand or you exile two cards with the same name, whichever comes first"];
}
