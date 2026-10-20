namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent transforming into a described form: "transforms into a 0/0 phyrexian artifact creature" (incubate's reminder).</summary>
/// <exampledoc>Sculpted Perfection</exampledoc>
/// <examplecapture>transforms into a 0/0 phyrexian artifact creature</examplecapture>
[Dependent]
public class TransformsInto : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("transforms?"), "into", Pattern("an?"), Prop(Stats), Prop(Subtypes), Prop(Types)];

    public PowerToughness Stats { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
}
