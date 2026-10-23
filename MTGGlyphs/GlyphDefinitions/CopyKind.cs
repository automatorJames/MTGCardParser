namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a renamed copy also is, after "it's"/"he's": "legendary in addition to its other types", "a 4/4 spider human hero in addition to his other types", "a legendary human mercenary villain creature".</summary>
/// <exampledoc>Sarkhan, Soul Aflame</exampledoc>
/// <examplecapture>legendary in addition to its other types</examplecapture>
[Dependent]
public class CopyKind : Glyph
{
    public override Nib[] Nibs => [Opt(Pattern("an?")), Prop(Legendary), Prop(Stats), Prop(Subtypes), Prop(Types), Prop(Additional)];

    [RegexPattern("legendary")]
    public bool Legendary { get; set; }
    [Optional]
    public PowerToughness Stats { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Subtypes { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
    [Optional]
    public CopyTypeAddition? Additional { get; set; }
}
