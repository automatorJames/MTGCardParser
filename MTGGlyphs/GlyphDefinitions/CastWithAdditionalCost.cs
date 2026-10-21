namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Casting a card from a zone by paying an extra cost: "cast this card from your graveyard by discarding a card in addition to paying its other costs" (jump-start, retrace), "… by paying 3 life and discarding a card …".</summary>
/// <exampledoc>Maximize Velocity</exampledoc>
/// <examplecapture>cast this card from your graveyard by discarding a card in addition to paying its other costs</examplecapture>
[Dependent]
public class CastWithAdditionalCost : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Cast), "by", Prop(Payment), "in addition to paying its other costs"];

    public PlayFromZone Cast { get; set; }
    public OneOf<ManyOf<Cost>, Cost> Payment { get; set; }
}
