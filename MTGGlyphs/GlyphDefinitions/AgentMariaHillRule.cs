namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Agent Maria Hill, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Agent Maria Hill</exampledoc>
[TokenizationOrder(-1)]
public class AgentMariaHillRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever", Nib.This, "becomes tapped to pay a teamwork cost, put a +1/+1 counter on her and draw a card"];
}
