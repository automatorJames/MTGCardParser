namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent becoming a creature, or another kind of creature, for a time (animation, transformation): "becomes a 4/4 red and green dragon artifact creature with flying until end of turn", "become a 3/3 elemental creature with haste until end of turn", "becomes an artifact creature until end of turn", "becomes a blue frog with base power and toughness 1/1", "becomes an artifact in addition to its other types until end of turn". The description reads like a token's: power and toughness, colors, subtypes, then card types and/or base stats, then keywords, "that's still a land" when an animated land keeps its type, or "in addition to its other types".</summary>
/// <exampledoc>Azorius Keyrune</exampledoc>
/// <examplecapture>becomes a 2/2 white and blue bird artifact creature with flying until end of turn</examplecapture>
public class BecomesCreature : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Stats), Prop(Colors), Prop(Subtypes), Prop(Types), Prop(With), Prop(StillALand), Prop(InAddition), Prop(Duration)];

    public AnimateVerb Verb { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    [Optional]
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    public OneOf<AnimatedTypes, WithBaseStats> Types { get; set; }
    [Optional]
    public WithKeywords With { get; set; }
    [RegexPattern("that's still a land")]
    public bool StillALand { get; set; }
    [RegexPattern("in addition to (its|their) other types")]
    public bool InAddition { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
