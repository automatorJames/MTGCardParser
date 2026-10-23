namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How one player outnumbers another, said of a player in a relative clause: "controls more creatures", "has more life", "has more cards in hand".</summary>
/// <exampledoc>Oath of Druids</exampledoc>
/// <examplecapture>controls more creatures</examplecapture>
[Dependent]
public class PlayerMeasure : GlyphOneOf
{
    public ControlsCount Controls { get; set; }
    public HasLifeCompared Life { get; set; }
    public HasCardsInHandCompared Cards { get; set; }
}
