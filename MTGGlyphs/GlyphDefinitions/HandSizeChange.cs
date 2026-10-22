namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A change to a maximum hand size: "reduced by one", "increased by two".</summary>
/// <exampledoc>Thought Nibbler</exampledoc>
/// <examplecapture>reduced by two</examplecapture>
[Dependent]
public class HandSizeChange : Glyph
{
    public override Nib[] Nibs => [Prop(Direction), "by", Prop(Amount)];

    public HandSizeChangeDirection Direction { get; set; }
    public Quantity Amount { get; set; }
}
