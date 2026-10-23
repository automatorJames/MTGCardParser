namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The card types a permanent becomes, with any base stats it gets: "artifact creature", "artifact creature with base power and toughness 5/5", "artifact creature with power and toughness each equal to its mana value", or plural for several permanents: "artifact creatures".</summary>
/// <exampledoc>Tezzeret, Agent of Bolas</exampledoc>
/// <examplecapture>artifact creature with base power and toughness 5/5</examplecapture>
[Dependent]
public class AnimatedTypes : Glyph
{
    public override Nib[] Nibs => [Plural(Prop(Types)), Prop(Base)];

    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
    [Optional]
    public OneOf<WithBaseStats, WithStatsEqualTo> Base { get; set; }
}
