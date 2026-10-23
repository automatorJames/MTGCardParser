namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount computed from two others: "the number of cards in their hand minus 4", "3 minus the number of cards in their hand", "1 plus the sacrificed creature's mana value", "1 plus twice the number of age counters on it", "20 minus the highest life total among players".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>the number of cards in their hand minus 4</examplecapture>
[Dependent]
public class ArithmeticAmount : Glyph
{
    public override Nib[] Nibs => [Prop(Left), Prop(Operator), Prop(Right)];

    public OneOf<NumberOf, CharacteristicOf, Quantity?> Left { get; set; }
    public ArithmeticOperator Operator { get; set; }
    public OneOf<NumberOf, CharacteristicOf, OneOf<Quantity?, LifeTotalValue?, ScaledAmountExact>> Right { get; set; }
}
