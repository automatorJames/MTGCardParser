namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Gaining or losing an amount of life: "gain 2 life", "loses 1 life".</summary>
/// <exampledoc>Onulet</exampledoc>
/// <examplecapture>gain 2 life</examplecapture>
[Dependent]
public class GainOrLoseLife : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(GainOrLose), Prop(Quantity), "life"];

    public GainOrLose GainOrLose { get; set; }
    public Quantity Quantity { get; set; }
}
