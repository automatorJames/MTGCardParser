namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"there are four or more card types among cards in your graveyard" (delirium): how many card types a zone's cards span.</summary>
/// <exampledoc>Gnarlwood Dryad</exampledoc>
/// <examplecapture>there are four or more card types among cards in your graveyard</examplecapture>
[Dependent]
public class CardTypesCountCondition : Glyph
{
    public override Nib[] Nibs => ["there are", Prop(Count), "card types among cards in", Prop(Zone)];

    public AtLeastCount Count { get; set; }
    public PlayersZone Zone { get; set; }
}
