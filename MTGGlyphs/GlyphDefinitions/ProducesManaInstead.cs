namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A mana-replacement effect: what a land tapped for mana produces instead: "it produces {b} instead of any other type", "it produces colorless mana instead of any other type and amount".</summary>
/// <exampledoc>Infernal Darkness</exampledoc>
/// <examplecapture>produces {b} instead of any other type</examplecapture>
[Dependent]
public class ProducesManaInstead : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("produces?"), Prop(Mana), "instead of any other type", Prop(AndAmount)];

    public OneOf<ManaCost, ProducedManaKind?> Mana { get; set; }
    [Optional]
    [RegexPattern("and amount")]
    public bool? AndAmount { get; set; }
}
