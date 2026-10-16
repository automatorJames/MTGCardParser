namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Gaining or losing an amount of life: "gain 2 life", "loses 1 life", "gains life equal to its power", "gain that much life". Also the second half of "draws two cards and loses 2 life".</summary>
/// <exampledoc>Onulet</exampledoc>
/// <examplecapture>gain 2 life</examplecapture>
[Dependent]
public class GainOrLoseLife : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(GainOrLose), Prop(Amount)];

    public GainOrLose GainOrLose { get; set; }
    public OneOf<LifeQuantity, LifeEqualTo, ThatMuchLife> Amount { get; set; }
}
