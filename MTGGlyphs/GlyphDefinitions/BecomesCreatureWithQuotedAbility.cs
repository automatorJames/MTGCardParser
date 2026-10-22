namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent becoming a creature that has a quoted ability (creature lands, Genju): `becomes a 3/3 red and green elemental creature with "whenever this creature attacks, …`. Separate from <see cref="BecomesCreature"/> because a quoted ability is a dynamic slot, which can't be optional there; the closing quote falls after the period, so nothing follows it.</summary>
/// <exampledoc>Raging Ravine</exampledoc>
/// <examplecapture>becomes a 3/3 red and green elemental creature with "whenever this creature attacks, put a +1/+1 counter on it</examplecapture>
public class BecomesCreatureWithQuotedAbility : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Stats), Prop(Colors), Prop(Subtypes), Prop(Types), Prop(Quoted)];

    public AnimateVerb Verb { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    [Optional]
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    public AnimatedTypes Types { get; set; }
    public WithQuotedAbility Quoted { get; set; }
}
