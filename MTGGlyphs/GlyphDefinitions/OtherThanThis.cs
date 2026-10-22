namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"other than this creature", "other than {this}": excludes the object with the ability from a group or a choice.</summary>
/// <exampledoc>Demonic Taskmaster</exampledoc>
/// <examplecapture>other than this creature</examplecapture>
[Dependent]
public class OtherThanThis : Glyph
{
    public override Nib[] Nibs => ["other than", Prop(Self)];

    public OneOf<This, ThisCreature> Self { get; set; }
}
