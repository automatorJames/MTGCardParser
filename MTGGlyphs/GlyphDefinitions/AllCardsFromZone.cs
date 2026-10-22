namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Every card of a kind in a zone, moved at once: "all enchantment cards from your graveyard", "all cards from your graveyard", "all basic land cards from all graveyards".</summary>
/// <exampledoc>Replenish</exampledoc>
/// <examplecapture>all enchantment cards from your graveyard</examplecapture>
[Dependent]
public class AllCardsFromZone : Glyph
{
    public override Nib[] Nibs => ["all", Prop(Qualifiers), "cards from", Prop(Zone)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, QualifierRun, AndOrQualifiers> Qualifiers { get; set; }
    public PlayersZone Zone { get; set; }
}
