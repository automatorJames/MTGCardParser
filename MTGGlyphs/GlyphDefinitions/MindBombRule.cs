namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mind Bomb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mind Bomb</exampledoc>
[TokenizationOrder(-1)]
public class MindBombRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage to each player equal to 3 minus the number of cards they discarded this way"];
}
