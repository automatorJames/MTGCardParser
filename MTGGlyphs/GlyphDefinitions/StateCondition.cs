namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"unless a player has 13 or less life", "if you have 25 or more life", "unless there are seven or more cards in your graveyard", "as long as it's attacking", "if a nonland permanent left the battlefield this turn or a spell was warped this turn": a condition on the game state (or either of two) introduced by a connective.</summary>
/// <exampledoc>Murky Sewer</exampledoc>
/// <examplecapture>unless a player has 13 or less life</examplecapture>
[Dependent]
public class StateCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Connective), Prop(State)];

    public ConditionConnective Connective { get; set; }
    public OneOf<GameState, GameStateEither> State { get; set; }
}
