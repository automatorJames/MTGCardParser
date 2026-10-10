namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost that must be paid as well as casting a spell: "as an additional cost to cast this spell, sacrifice a creature".</summary>
/// <exampledoc>Metamorphosis</exampledoc>
/// <examplecapture>as an additional cost to cast this spell, sacrifice a creature</examplecapture>
public class AdditionalCastingCost : Glyph
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell,", Prop(Cost)];

    [AllowUnmatched]
    public DynamicGlyph Cost { get; set; }
}
