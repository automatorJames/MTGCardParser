namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One mode of a pawprint modal spell, with its pawprint cost: "{p}{p} — exile target nonland permanent".</summary>
/// <exampledoc>Season of the Burrow</exampledoc>
/// <examplecapture>{p}{p} — exile target nonland permanent</examplecapture>
public class PawprintMode : Glyph
{
    public override Nib[] Nibs => [Prop(Cost), "—", Prop(Effect)];

    public Pawprints Cost { get; set; }
    public DynamicGlyph Effect { get; set; }
}
