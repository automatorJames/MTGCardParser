namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flash of Insight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flash of Insight</exampledoc>
[TokenizationOrder(-1)]
public class FlashOfInsightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't exile", Nib.This, "to pay for its own flashback cost"];
}
