namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A regeneration effect: "regenerate {this}", "regenerate target creature" - sets up a regeneration shield for one permanent.</summary>
/// <exampledoc>Drudge Skeletons</exampledoc>
/// <examplecapture>regenerate {this}</examplecapture>
public class Regenerate : Glyph
{
    public override Nib[] Nibs => ["regenerate", Prop(Regenerated)];

    public PermanentPhrase Regenerated { get; set; }
}
