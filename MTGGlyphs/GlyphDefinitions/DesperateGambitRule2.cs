namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Desperate Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desperate Gambit</exampledoc>
[TokenizationOrder(-1)]
public class DesperateGambitRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win the flip, the next time that source would deal damage this turn, it deals double that damage instead"];
}
