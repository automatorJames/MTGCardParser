namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An activated ability: its costs, a colon, and what it does - "{2}, {t}: target creature gains flying until end of turn".</summary>
/// <exampledoc>Flying Carpet</exampledoc>
/// <examplecapture>{2}, {t}: target creature gains flying until end of turn</examplecapture>
public class ActivatedAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Costs), ":", Prop(Effect)];

    public CompoundOf<Cost> Costs { get; set; }

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
