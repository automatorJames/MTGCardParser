namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a qualifier word can be: a supertype ("legendary", "snow"), a card type, a color, how many colors ("multicolored"), tapped or untapped, a status or designation ("face-down", "suspected", "modified"), a combat state ("attacking"), a creature type ("non-wall"), a planeswalker type ("rowan"), another subtype ("saga", "case", "adventure"), being a token ("nontoken"), a rules-defined group of cards ("historic", "outlaw") or a spell's status ("kicked").</summary>
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
    public PermanentDesignation? Designation { get; set; }
    public CombatState? CombatState { get; set; }
    public CreatureType? CreatureType { get; set; }
    public PlaneswalkerType? PlaneswalkerType { get; set; }
    public PermanentSubtype? Subtype { get; set; }
    public TokenStatus? TokenStatus { get; set; }
    public CardGroup? CardGroup { get; set; }
    public SpellStatus? SpellStatus { get; set; }
}
