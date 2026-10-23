namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Split the Party, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Split the Party</exampledoc>
[TokenizationOrder(-1)]
public class SplitThePartyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return half the creatures they control to their owner's hand, rounded up"];
}
