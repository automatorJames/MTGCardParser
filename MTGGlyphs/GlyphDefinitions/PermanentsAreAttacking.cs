namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Creatures attacking, as a condition: "four or more creatures are attacking", "a white creature is attacking", "a black creature with flying is attacking".</summary>
/// <exampledoc>Lethargy Trap</exampledoc>
/// <examplecapture>three or more creatures are attacking</examplecapture>
[Dependent]
public class PermanentsAreAttacking : Glyph
{
    public override Nib[] Nibs => [Prop(What), Alt("is", "are"), "attacking"];

    public OneOf<PermanentsThreshold, IndefinitePermanent> What { get; set; }
}
