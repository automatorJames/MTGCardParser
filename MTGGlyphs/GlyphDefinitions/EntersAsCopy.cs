namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A clone replacement: "you may have {this} enter the battlefield as a copy of any creature on the battlefield", with any exceptions held unresolved.</summary>
/// <exampledoc>Clone</exampledoc>
/// <examplecapture>you may have {this} enter the battlefield as a copy of any creature on the battlefield</examplecapture>
public class EntersAsCopy : Glyph
{
    public override Nib[] Nibs => ["you may have", Nib.This, "enter the battlefield as a copy of", Prop(Copied)];

    [AllowUnmatched]
    public DynamicGlyph Copied { get; set; }
}
