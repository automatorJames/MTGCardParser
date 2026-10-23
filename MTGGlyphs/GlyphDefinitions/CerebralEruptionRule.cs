namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cerebral Eruption, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cerebral Eruption</exampledoc>
[TokenizationOrder(-1)]
public class CerebralEruptionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage equal to the revealed card's mana value to that player and each creature that player controls"];
}
