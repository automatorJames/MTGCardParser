namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by artifact creatures": the sources a prevention or redirection effect applies to.</summary>
/// <exampledoc>Argothian Pixies</exampledoc>
/// <examplecapture>by artifact creatures</examplecapture>
[Dependent]
public class DamageSource : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Sources)];

    public Permanents Sources { get; set; }
}
