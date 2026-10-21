namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The card types a permanent becomes, with any base stats it gets: "artifact creature", "artifact creature with base power and toughness 5/5".</summary>
/// <exampledoc>Tezzeret, Agent of Bolas</exampledoc>
/// <examplecapture>artifact creature with base power and toughness 5/5</examplecapture>
[Dependent]
public class AnimatedTypes : Glyph
{
    public override Nib[] Nibs => [Prop(Types), Prop(Base)];

    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> Types { get; set; }
    [Optional]
    public WithBaseStats Base { get; set; }
}
