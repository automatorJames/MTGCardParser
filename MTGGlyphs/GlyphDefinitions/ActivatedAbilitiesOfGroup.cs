namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"activated abilities of creatures you control": the activated abilities of a group of permanents, as what a cost change applies to.</summary>
/// <exampledoc>Training Grounds</exampledoc>
/// <examplecapture>activated abilities of creatures you control</examplecapture>
[Dependent]
public class ActivatedAbilitiesOfGroup : Glyph
{
    public override Nib[] Nibs => ["activated abilities of", Prop(Group)];

    public Permanents Group { get; set; }
}
