namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A named legendary token, created by name: "ragavan, a legendary 2/1 red monkey creature token" - the token's name, then its description as in <see cref="TokenDescription"/> (stats, colors, subtypes, types, keywords).</summary>
/// <exampledoc>Ragavan, Nimble Pilferer</exampledoc>
/// <examplecapture>ragavan, a legendary 2/1 red monkey creature token</examplecapture>
[Dependent]
public class NamedLegendaryToken : Glyph
{
    public override Nib[] Nibs => [Pattern("[a-z][a-z' -]*"), ", a legendary", Prop(Stats), Prop(Colors), Prop(Subtypes), Prop(Types), "token", Prop(With)];

    [Optional]
    public PowerToughness Stats { get; set; }
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
    [Optional]
    public OneOf<WithKeywords, WithKeywordAbility> With { get; set; }
}
