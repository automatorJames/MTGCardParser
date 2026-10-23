namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"other than this creature", "other than {this}", "other than enchanted creature", "other than that creature": excludes the object with the ability, the permanent it's attached to, or one named earlier, from a group or a choice.</summary>
/// <exampledoc>Demonic Taskmaster</exampledoc>
/// <examplecapture>other than this creature</examplecapture>
[Dependent]
public class OtherThanThis : Glyph
{
    public override Nib[] Nibs => ["other than", Prop(Self)];

    public OneOf<This, ThisCreature, OneOf<EnchantedPermanent, ThatCard>> Self { get; set; }
}
