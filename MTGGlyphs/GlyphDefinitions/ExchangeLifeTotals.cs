namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exchanging life totals between players: "exchange life totals with target opponent", "two target players exchange life totals".</summary>
/// <exampledoc>Mirror Universe</exampledoc>
/// <examplecapture>exchange life totals with target opponent</examplecapture>
public class ExchangeLifeTotals : Glyph, IPredicate
{
    public override Nib[] Nibs => [Alt("exchange", "exchanges"), "life totals", Prop(With)];

    [Optional]
    public WithPlayer With { get; set; }
}
