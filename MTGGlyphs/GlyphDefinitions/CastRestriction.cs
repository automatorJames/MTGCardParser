namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A restriction on when a spell may be cast: "cast this spell only during the declare blockers step", "cast this spell only during your declare attackers step".</summary>
/// <exampledoc>False Orders</exampledoc>
/// <examplecapture>cast this spell only during the declare blockers step</examplecapture>
public class CastRestriction : Glyph
{
    public override Nib[] Nibs => ["cast this spell only", Prop(Timing)];

    [AllowUnmatched]
    public DynamicGlyph Timing { get; set; }
}
