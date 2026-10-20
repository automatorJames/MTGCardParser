namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the life lost this way", "the damage prevented this way", "the damage dealt this way": an amount measured by the effect just described.</summary>
/// <exampledoc>Blood Tithe</exampledoc>
/// <examplecapture>the life lost this way</examplecapture>
[Dependent]
public class AmountThisWay : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Kind), "this way"];

    public ThisWayAmount Kind { get; set; }
}
