namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The creatures the "bands with other" damage rule applies to: "at least two legendary creatures you control, one of which has "bands with other legendary creatures,"".</summary>
/// <exampledoc>Cathedral of Serra</exampledoc>
/// <examplecapture>at least two legendary creatures you control, one of which has "bands with other legendary creatures,"</examplecapture>
[Dependent]
public class BandsWithOtherCreatures : Glyph
{
    public override Nib[] Nibs => ["at least two", Prop(Group), Prop(Controller), ", one of which has", Prop(Ability), ",\""];

    public BandGroup Group { get; set; }
    public ControlledBy Controller { get; set; }
    public BandsWithOther Ability { get; set; }
}
