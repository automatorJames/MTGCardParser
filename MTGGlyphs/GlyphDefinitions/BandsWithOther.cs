namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The "bands with other" keyword, naming the group a creature can band with: "bands with other legendary creatures".</summary>
/// <exampledoc>Cathedral of Serra</exampledoc>
/// <examplecapture>"bands with other legendary creatures</examplecapture>
[Dependent]
public class BandsWithOther : Glyph
{
    public override Nib[] Nibs => ["\"bands with other", Prop(Group)];

    public BandGroup Group { get; set; }
}
