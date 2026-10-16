namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The second half of a combined cost: "and exile a blue card from your hand".</summary>
/// <exampledoc>Force of Will</exampledoc>
/// <examplecapture>and exile a blue card from your hand</examplecapture>
[Dependent]
public class AndCost : Glyph
{
    public override Nib[] Nibs => ["and", Prop(Cost)];

    public Cost Cost { get; set; }
}
