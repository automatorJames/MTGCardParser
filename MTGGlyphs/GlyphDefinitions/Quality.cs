namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a qualifier word can be: a card type, a color, tapped or untapped, or a combat state ("attacking").</summary>
/// <exampledoc>Desert</exampledoc>
/// <examplecapture>attacking</examplecapture>
[Dependent]
public class Quality : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public ManaColor? ManaColor { get; set; }
    public TapState? TapState { get; set; }
    public CombatState? CombatState { get; set; }
}
