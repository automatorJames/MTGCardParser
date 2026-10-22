namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"spider hero", "bear berserker", "1/1 spirit", "4/4 hero": creature types a permanent becomes or gains, several in a row, possibly with base power and toughness first. Only creature types, so "artifact creature" stays with <see cref="BecomesCreature"/>.</summary>
/// <exampledoc>Spider-Suit</exampledoc>
/// <examplecapture>spider hero</examplecapture>
[Dependent]
public class DescribedKinds : Glyph
{
    public override Nib[] Nibs => [Prop(Stats), Prop(Kinds)];

    [Optional]
    public PowerToughness Stats { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Kinds { get; set; }
}
