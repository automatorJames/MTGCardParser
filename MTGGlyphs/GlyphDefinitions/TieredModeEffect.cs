namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A tiered mode: a flavor name, the extra cost to choose that mode, then its effect: "fire — {0} — {this} deals 1 damage to each creature", "cura — {1} — target permanent gains …". The name has no rules meaning; the cost is paid in addition for choosing the mode (tiered).</summary>
/// <exampledoc>Fire Magic</exampledoc>
/// <examplecapture>fira — {2} — {this} deals 2 damage to each creature</examplecapture>
[Dependent]
public class TieredModeEffect : Glyph
{
    public override Nib[] Nibs => [Pattern("[a-z][a-z'!-]*( [a-z][a-z'!-]*){0,3}"), "—", Prop(Cost), "—", Prop(Effect)];

    public ManaCost Cost { get; set; }
    public DynamicGlyph Effect { get; set; }
}
