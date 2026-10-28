namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect done only if a condition holds, the condition trailing: "put that card into your hand if it's a land", "put a +1/+1 counter on it if it's a creature", "untap all creatures you control if {w} was spent to cast this spell", "draw a card if you control a creature with a +1/+1 counter on it", "counter target spell if it was kicked", and replacements via <see cref="PredicateInstead"/>: "{this} deals 4 damage instead if you control three or more artifacts", "{this} deals 3 damage instead if a nonland permanent left the battlefield this turn or a spell was warped this turn", "it deals 4 damage to that creature instead if this spell's additional cost was paid". Tried after other glyphs, so predicates with their own trailing conditions keep them.</summary>
/// <exampledoc>Dire Fleet Interloper</exampledoc>
/// <examplecapture>put that card into your hand if it's a land</examplecapture>
[TokenizationOrder(-1)]
public class EffectIfCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Effect), "if", Prop(Condition)];

    [Optional]
    public Subject Subject { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Effect { get; set; }
    public OneOf<CardIsKind, TransformedType, ManaWasSpent, GameState, ControlsPermanent, KeywordCostWasPaid, WasKicked, GameStateEither> Condition { get; set; }
}
