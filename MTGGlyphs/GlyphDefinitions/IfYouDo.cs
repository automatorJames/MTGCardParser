namespace MTGGlyphs.GlyphDefinitions;

public class IfYouDo : Glyph
{
    public override Nib[] Nibs => ["if you do,", Prop(Outcome)];

    [AllowUnmatched]
    public DynamicGlyph Outcome { get; set; }
}
