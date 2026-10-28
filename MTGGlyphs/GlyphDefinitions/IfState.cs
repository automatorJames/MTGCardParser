namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on the game state, checked as an "if": after a static ability it limits when the ability applies ("if it has a depletion counter on it"); in an activation restriction it's "only if enchanted land is untapped", "only if you control a snow mountain", "only if there are no charge counters on {this}", "only if there are seven or more cards in your graveyard", "only if you attacked this turn", "only if this creature attacked this turn".</summary>
/// <exampledoc>Land Cap</exampledoc>
/// <examplecapture>if it has a depletion counter on it</examplecapture>
[Dependent]
public class IfState : Glyph
{
    public override Nib[] Nibs => [Opt("only"), "if", Prop(Condition)];

    public OneOf<PermanentIs, HasCounters, ControlsPermanent, CountersThere, CardCountCondition, HappenedThisTurn, CardTypesCountCondition, CombatedThisTurn> Condition { get; set; }
}
