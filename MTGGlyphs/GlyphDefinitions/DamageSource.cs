namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by artifact creatures", "by artifact sources", "by target creature", "by that creature", "by a source of your choice": the sources a prevention or redirection effect applies to - a group, one particular permanent, or a source the player picks.</summary>
/// <exampledoc>Argothian Pixies</exampledoc>
/// <examplecapture>by artifact creatures</examplecapture>
[Dependent]
public class DamageSource : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Sources)];

    public OneOf<Permanents, DamageSources, OneOf<PermanentPhrase, ChosenSource>> Sources { get; set; }
}
