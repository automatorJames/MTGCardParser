namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Temur Battle Rage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Temur Battle Rage</exampledoc>
[TokenizationOrder(-1)]
public class TemurBattleRageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ferocious — that creature also gains trample until end of turn if you control a creature with power 4 or greater"];
}
