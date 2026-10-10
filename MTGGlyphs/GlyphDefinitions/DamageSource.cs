namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by artifact creatures", "by artifact sources": the sources a prevention or redirection effect applies to.</summary>
/// <exampledoc>Argothian Pixies</exampledoc>
/// <examplecapture>by artifact creatures</examplecapture>
[Dependent]
public class DamageSource : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Sources)];

    public OneOf<Permanents, DamageSources> Sources { get; set; }
}
