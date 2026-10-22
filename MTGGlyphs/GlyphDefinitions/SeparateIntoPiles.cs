namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Separating cards or permanents into piles, as the first half of a pile-choice effect: "an opponent separates those cards into two piles", "separate all creatures target player controls into two piles", "(reveal …) and separate them into two piles".</summary>
/// <exampledoc>Fact or Fiction</exampledoc>
/// <examplecapture>separates those cards into two piles</examplecapture>
public class SeparateIntoPiles : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("separates?"), Prop(Separated), "into", Prop(Piles)];

    public OneOf<PermanentPhrase, RemovedObjects> Separated { get; set; }
    public Piles Piles { get; set; }
}
