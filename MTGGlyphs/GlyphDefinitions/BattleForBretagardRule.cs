namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Battle for Bretagard, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Battle for Bretagard</exampledoc>
[TokenizationOrder(-1)]
public class BattleForBretagardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["iii — choose any number of artifact tokens and/or creature tokens you control with different names"];
}
