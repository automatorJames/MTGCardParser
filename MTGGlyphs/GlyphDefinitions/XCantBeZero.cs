namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A restriction on the value chosen for x in a cost: "x can't be 0".</summary>
/// <exampledoc>Ertai's Meddling</exampledoc>
/// <examplecapture>x can't be 0</examplecapture>
public class XCantBeZero : Glyph
{
    public override Nib[] Nibs => ["x can't be 0"];
}
