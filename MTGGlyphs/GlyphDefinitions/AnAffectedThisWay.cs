namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a creature destroyed this way": one of the objects an effect just acted on, with an article.</summary>
/// <exampledoc>Essence Vortex</exampledoc>
/// <examplecapture>a creature destroyed this way</examplecapture>
[Dependent]
public class AnAffectedThisWay : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Affected)];

    public AffectedThisWay Affected { get; set; }
}
