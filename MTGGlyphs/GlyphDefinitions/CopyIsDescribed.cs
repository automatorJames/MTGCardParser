namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A copy's changed characteristics: "except it's a 4/4 black zombie jackal warrior with no mana cost", "except it's an enchantment in addition to its other types", "except it's 7/7". Read like a token description: power and toughness, colors, subtypes, card types, whether they're added to its other types, and whether it loses its mana cost.</summary>
/// <exampledoc>Earthshaker Khenra</exampledoc>
/// <examplecapture>except it's a 4/4 black zombie jackal warrior with no mana cost</examplecapture>
[Dependent]
public class CopyIsDescribed : Glyph
{
    public override Nib[] Nibs => [Prop(Article), Prop(Stats), Prop(Colors), Prop(Subtypes), Prop(Types), Prop(Additional), Prop(NoManaCost)];

    public CopyArticle Article { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    [Optional]
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    [Optional]
    public CardType? Types { get; set; }
    [RegexPattern("in addition to its other types")]
    public bool Additional { get; set; }
    [RegexPattern("with no mana cost")]
    public bool NoManaCost { get; set; }
}
