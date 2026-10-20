namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the greatest power among creatures you control", "the greatest mana value among permanents you control": the highest value of a characteristic in a group.</summary>
/// <exampledoc>The Great Henge</exampledoc>
/// <examplecapture>the greatest power among creatures you control</examplecapture>
[Dependent]
public class GreatestAmong : Glyph
{
    public override Nib[] Nibs => ["the greatest", Prop(Characteristic), "among", Prop(Group)];

    public Characteristic Characteristic { get; set; }
    public Permanents Group { get; set; }
}
