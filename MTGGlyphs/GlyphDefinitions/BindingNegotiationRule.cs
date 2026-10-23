namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Binding Negotiation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Binding Negotiation</exampledoc>
[TokenizationOrder(-1)]
public class BindingNegotiationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, you may put a face-up exiled card they own into their graveyard"];
}
