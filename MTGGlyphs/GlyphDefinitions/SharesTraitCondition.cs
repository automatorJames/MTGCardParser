namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object sharing a characteristic with another, as a condition: "it shares a creature type with this creature" (kinship), "it shares a color with that permanent".</summary>
/// <exampledoc>Ink Dissolver</exampledoc>
/// <examplecapture>it shares a creature type with this creature</examplecapture>
[Dependent]
public class SharesTraitCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Pattern("shares?"), "a", Prop(Trait), "with", Prop(Other)];

    public OneOf<It, PermanentPhrase> Object { get; set; }
    public SharedTrait Trait { get; set; }
    public PermanentPhrase Other { get; set; }
}
