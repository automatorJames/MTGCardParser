namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement effect applied as {this} enters the battlefield: "as {this} enters the battlefield, choose an opponent".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>as {this} enters the battlefield, choose an opponent</examplecapture>
public class AsThisEnters : Glyph
{
    public override Nib[] Nibs => ["as", Nib.This, "enters the battlefield,", Prop(Effect)];

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
