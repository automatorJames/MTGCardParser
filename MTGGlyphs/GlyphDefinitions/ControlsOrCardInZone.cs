namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you control a desert or there is a desert card in your graveyard": the desert cards' condition, either a permanent controlled or a card in a zone.</summary>
/// <exampledoc>Sidewinder Naga</exampledoc>
/// <examplecapture>you control a desert or there is a desert card in your graveyard</examplecapture>
[Dependent]
public class ControlsOrCardInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Controls), "or", Prop(CardInZone)];

    public PlayerControls Controls { get; set; }
    public CardInZoneCondition CardInZone { get; set; }
}
