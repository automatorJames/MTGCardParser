namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A leaves-the-battlefield trigger: "when {this} leaves the battlefield, …".</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>when {this} leaves the battlefield, that creature's controller sacrifices it</examplecapture>
public class WhenThisLeavesTheBattlefield : Glyph
{
    public override Nib[] Nibs => ["when", Nib.This, "leaves the battlefield,", Prop(Result)];

    public DynamicGlyph Result { get; set; }
}