namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What collecting evidence does (its reminder text): "exile cards with total mana value 6 or greater from your graveyard".</summary>
/// <exampledoc>Extract a Confession</exampledoc>
/// <examplecapture>exile cards with total mana value 6 or greater from your graveyard</examplecapture>
public class CollectEvidenceRule : Glyph, IPredicate
{
    public override Nib[] Nibs => ["exile cards with total mana value", Prop(Amount), "or greater", Prop(From)];

    public Quantity Amount { get; set; }
    public FromZone From { get; set; }
}
