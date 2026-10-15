namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The attack rule of "bands with other", from its reminder text: "any legendary creatures can attack in a band as long as at least one has "bands with other legendary creatures"".</summary>
/// <exampledoc>Cathedral of Serra</exampledoc>
/// <examplecapture>any legendary creatures can attack in a band as long as at least one has "bands with other legendary creatures</examplecapture>
public class BandsWithOtherAttackRule : Glyph
{
    public override Nib[] Nibs => ["any", Prop(Group), "can attack in a band as long as at least one has", Prop(Ability)];

    public BandGroup Group { get; set; }
    public BandsWithOther Ability { get; set; }
}
