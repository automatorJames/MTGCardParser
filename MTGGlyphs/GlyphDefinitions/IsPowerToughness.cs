namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Setting a creature's base power and toughness, as a static predicate: "(enchanted creature) is 1/1" (the cursed Role).</summary>
/// <exampledoc>Spiteful Hexmage</exampledoc>
/// <examplecapture>is 1/1</examplecapture>
[Dependent]
public class IsPowerToughness : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["is", Prop(Stats)];

    public PowerToughness Stats { get; set; }
}
