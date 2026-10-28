namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A copy's changed characteristics: "except it's a 4/4 black zombie jackal warrior with no mana cost", "except it's an enchantment in addition to its other types", "except it's 7/7", "except it's a 1/1 white spirit creature with flying in addition to its other types". Read like a token description: power and toughness, colors, subtypes, card types, keywords, whether they're added to its other types (or colors and types, or creature types), and whether it loses its mana cost. "and it's a …" continues a list of changes (CopyChangePair).</summary>
/// <exampledoc>Earthshaker Khenra</exampledoc>
/// <examplecapture>except it's a 4/4 black zombie jackal warrior with no mana cost</examplecapture>
[Dependent]
public class CopyIsDescribed : Glyph
{
    public override Nib[] Nibs => [Prop(Article), Prop(Stats), Prop(Colors), Prop(Subtypes), Prop(Types), Prop(With), Prop(Additional), Prop(NoManaCost)];

    public CopyArticle Article { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    [Optional]
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
    [Optional]
    public OneOf<WithKeywords, WithKeywordAbility> With { get; set; }
    [Optional]
    public CopyTypeAddition? Additional { get; set; }
    [RegexPattern("with no mana cost")]
    public bool NoManaCost { get; set; }
}
