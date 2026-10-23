namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition comparing a creature's power or toughness with this one's, as evolve does: "that creature has greater power or toughness than this creature", "it has greater power or toughness than hulkling". It lets evolve's reminder text parse as the trigger it is.</summary>
/// <exampledoc>Evolving Adaptive</exampledoc>
/// <examplecapture>that creature has greater power or toughness than this creature</examplecapture>
[Dependent]
public class HasGreaterStatsThan : Glyph
{
    public override Nib[] Nibs => [Prop(Object), "has greater", Prop(Stats), "than", Prop(Other)];

    public OneOf<ThatCard, It> Object { get; set; }
    public PowerAndOrToughness Stats { get; set; }
    public OneOf<ThisCreature, This, ShortName> Other { get; set; }
}
