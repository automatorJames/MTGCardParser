namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Paying an amount of life or mana the player chooses: "pay any amount of life", "(each player may) pay any amount of mana". Narrow on purpose: a general "pay {cost}" predicate would take OptionalPayCost's "you may pay {1}" lines; the amount paid is referred back to later ("that many", "the amount of mana they paid this way").</summary>
/// <exampledoc>Minion of the Wastes</exampledoc>
/// <examplecapture>pay any amount of life</examplecapture>
public class PayAnyAmount : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("pays?"), "any amount of", Prop(Resource)];

    public PaidResource Resource { get; set; }
}
