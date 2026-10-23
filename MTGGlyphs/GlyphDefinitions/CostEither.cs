namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A choice of two costs, either of which pays: "sacrifice a nonland permanent of their choice or discard a card", "discards a card or pays {2}", "forage or pay {b}" (the second cost's paying verb said).</summary>
/// <exampledoc>Nicol Bolas, the Deceiver</exampledoc>
/// <examplecapture>sacrifices a nonland permanent of their choice or discards a card</examplecapture>
[Dependent]
public class CostEither : Glyph
{
    public override Nib[] Nibs => [Prop(First), "or", Opt(Pattern("pays?")), Prop(Second)];

    public Cost First { get; set; }
    public Cost Second { get; set; }
}
