namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"half their life, rounded up", "half your life, rounded up": half a player's life total with its rounding, as life to lose or pay. The comma is a literal, since a nested glyph can't open with one.</summary>
/// <exampledoc>Blood Tribute</exampledoc>
/// <examplecapture>half their life, rounded up</examplecapture>
[Dependent]
public class HalfLifeTotal : Glyph
{
    public override Nib[] Nibs => [Prop(Factor), Prop(Whose), "life", ",", Prop(Rounded)];

    public ScaleFactor Factor { get; set; }
    public Whose Whose { get; set; }
    public Rounding Rounded { get; set; }
}
