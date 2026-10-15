namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by artifact creatures", "by artifact sources", "by target creature", "by that creature": the sources a prevention or redirection effect applies to - a group, or one particular permanent.</summary>
/// <exampledoc>Argothian Pixies</exampledoc>
/// <examplecapture>by artifact creatures</examplecapture>
[Dependent]
public class DamageSource : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Sources)];

    public OneOf<Permanents, DamageSources, PermanentPhrase> Sources { get; set; }
}
