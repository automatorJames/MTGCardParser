namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Swallow Whole, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swallow Whole</exampledoc>
[TokenizationOrder(-1)]
public class SwallowWholeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on the creature tapped to pay this spell's additional cost"];
}
