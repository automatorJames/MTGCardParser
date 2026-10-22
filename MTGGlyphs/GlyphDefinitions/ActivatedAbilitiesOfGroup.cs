namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"activated abilities of creatures you control", "activated abilities of artifacts", "activated abilities of enchanted creature", "activated abilities of sources with the chosen name", "activated abilities of the chosen permanent": the activated abilities of the attached permanent, a group of permanents (or one named earlier), as what a cost change or a ban on activation applies to.</summary>
/// <exampledoc>Training Grounds</exampledoc>
/// <examplecapture>activated abilities of creatures you control</examplecapture>
[Dependent]
public class ActivatedAbilitiesOfGroup : Glyph
{
    public override Nib[] Nibs => ["activated abilities of", Prop(Group)];

    public OneOf<EnchantedPermanent, Permanents, OneOf<NamedSources, PermanentPhrase>> Group { get; set; }
}
