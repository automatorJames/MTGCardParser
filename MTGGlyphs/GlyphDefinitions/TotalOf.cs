namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the total power of creatures you control": a characteristic summed over a group.</summary>
/// <exampledoc>Ghalta, Primal Hunger</exampledoc>
/// <examplecapture>the total power of creatures you control</examplecapture>
[Dependent]
public class TotalOf : Glyph
{
    public override Nib[] Nibs => ["the total", Prop(Characteristic), "of", Prop(Group)];

    public Characteristic Characteristic { get; set; }
    public Permanents Group { get; set; }
}
