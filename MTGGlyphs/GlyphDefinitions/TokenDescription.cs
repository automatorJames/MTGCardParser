namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A description of tokens to create: how many, whether they enter tapped (or tapped and attacking), their power and toughness, colors, subtypes (absent for plain noncreature tokens: "a colorless artifact token named lotus"), card types, name and keywords (plain, or one with a parameter: "with protection from black", "with toxic 1"), and whether they enter attacking - "a 1/1 green saproling creature token", "three 0/1 black serf creature tokens", "a 3/1 black and red graveborn creature token with haste", "a tapped 1/1 blue fish creature token", "a tapped and attacking 1/1 red warrior creature token" (mobilize), "two 1/1 white soldier creature tokens that are tapped and attacking", "a 0/1 green wall creature token with defender named wood".</summary>
/// <exampledoc>Elvish Farmer</exampledoc>
/// <examplecapture>a 1/1 green saproling creature token</examplecapture>
[Dependent]
public class TokenDescription : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), Prop(Tapped), Prop(Stats), Prop(Colors), Prop(Subtypes), Prop(Types), Plural("token"), Prop(Name), Prop(With), Prop(NameAfterKeywords), Prop(Attacking)];

    public Quantity Quantity { get; set; }
    [Optional]
    public EntryState? Tapped { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
    [Optional]
    public TokenName Name { get; set; }
    [Optional]
    public OneOf<WithKeywords, WithKeywordAbility> With { get; set; }
    [Optional]
    public TokenName NameAfterKeywords { get; set; }
    [Optional]
    public EntersAttacking? Attacking { get; set; }
}
