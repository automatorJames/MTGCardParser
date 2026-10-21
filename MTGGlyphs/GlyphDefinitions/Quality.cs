namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a qualifier word can be: a supertype ("legendary", "snow"), a card type, a color, how many colors ("multicolored"), tapped or untapped, a combat state ("attacking"), a creature type ("non-wall") or being a token ("nontoken").</summary>
/// <exampledoc>Desert</exampledoc>
/// <examplecapture>attacking</examplecapture>
[Dependent]
public class Quality : GlyphOneOf
{
    public Supertype? Supertype { get; set; }
    public CardType? CardType { get; set; }
    public ManaColor? ManaColor { get; set; }
    public Colorfulness? Colorfulness { get; set; }
    public TapState? TapState { get; set; }
    public CombatState? CombatState { get; set; }
    public CreatureType? CreatureType { get; set; }
    public TokenStatus? TokenStatus { get; set; }
}
