namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chaos Wand, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chaos Wand</exampledoc>
[TokenizationOrder(-1)]
public class ChaosWandRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then put the exiled cards that weren't cast this way on the bottom of that library in a random order"];
}
