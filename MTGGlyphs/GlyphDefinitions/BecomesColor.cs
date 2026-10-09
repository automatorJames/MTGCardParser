namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A color-changing effect: "target spell or permanent becomes red" (the Lace cycle).</summary>
/// <exampledoc>Chaoslace</exampledoc>
/// <examplecapture>target spell or permanent becomes red</examplecapture>
public class BecomesColor : Glyph
{
    public override Nib[] Nibs => [Prop(Changed), "becomes", Prop(Color)];

    public TargetSpellOrPermanent Changed { get; set; }
    public ManaColor Color { get; set; }
}
