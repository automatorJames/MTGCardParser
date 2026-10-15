namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A description of tokens to create: how many, their power and toughness, colors, subtypes, card types and keywords - "a 1/1 green saproling creature token", "three 0/1 black serf creature tokens", "a 3/1 black and red graveborn creature token with haste".</summary>
/// <exampledoc>Elvish Farmer</exampledoc>
/// <examplecapture>a 1/1 green saproling creature token</examplecapture>
[Dependent]
public class TokenDescription : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(Stats), Prop(Colors), Prop(Subtypes), Prop(Types), Plural("token"), Prop(With)];

    public Quantity Quantity { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
    [Optional]
    public WithKeywords With { get; set; }
}
