namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"there is a desert card in your graveyard", "there's a creature card in your graveyard": a card of a kind being in a player's zone, as a condition.</summary>
/// <exampledoc>Ramunap Hydra</exampledoc>
/// <examplecapture>there is a desert card in your graveyard</examplecapture>
[Dependent]
public class CardInZoneCondition : Glyph
{
    public override Nib[] Nibs => [Prop(ThereIs), Pattern("an?"), Prop(Cards), Opt("is"), "in", Prop(Zone)];

    [RegexPattern("(there is|there's)")]
    public bool ThereIs { get; set; }
    public QualifiedCards Cards { get; set; }
    public PlayersZone Zone { get; set; }
}
