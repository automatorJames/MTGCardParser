namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Being a creature of a kind with set base stats, as a static effect (an aura's or a conditional one): "is a treefolk with base power and toughness 0/4", "is a creature with base power and toughness 5/5 in addition to its other types", "is a blue frog creature with base power and toughness 1/1". The "becomes" form is <see cref="BecomesCreature"/>; this one needs the base stats, so it doesn't read plain type checks ("is an enchantment").</summary>
/// <exampledoc>Ensoul Artifact</exampledoc>
/// <examplecapture>is a creature with base power and toughness 5/5 in addition to its other types</examplecapture>
[Dependent]
public class IsCreatureWithBaseStats : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("(is|are) an?"), Prop(Colors), Prop(Subtypes), Prop(Types), Prop(Base), Prop(InAddition)];

    [Optional]
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
    public WithBaseStats Base { get; set; }
    [RegexPattern("in addition to (its|their) other types")]
    public bool InAddition { get; set; }
}
