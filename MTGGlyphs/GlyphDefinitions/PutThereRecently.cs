namespace MTGGlyphs.GlyphDefinitions;

/// <summary>When a card arrived in its zone: "that was put there from the battlefield this turn", "that were put there from anywhere this turn", "that wasn't put there this turn" - a recency qualifier on cards outside the battlefield.</summary>
/// <exampledoc>Bonders' Enclave</exampledoc>
/// <examplecapture>that was put there from the battlefield this turn</examplecapture>
[Dependent]
public class PutThereRecently : Glyph
{
    public Arrival Arrival { get; set; }
    [Optional]
    public ArrivalOrigin? Origin { get; set; }
    public TurnSpan Span { get; set; }
}
