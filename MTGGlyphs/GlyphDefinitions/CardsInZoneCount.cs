namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the number of cards in your graveyard", "the number of creature cards in your graveyard", "the number of cards in its controller's graveyard": a count of cards (of one kind) in a zone, as a bound inside a with-qualifier (where the general <see cref="NumberOf"/> and <see cref="CardsInZone"/> would refer back to the qualifier itself).</summary>
/// <exampledoc>Drown in the Loch</exampledoc>
/// <examplecapture>the number of cards in its controller's graveyard</examplecapture>
[Dependent]
public class CardsInZoneCount : Glyph
{
    public override Nib[] Nibs => ["the number of", Prop(Qualifier), Plural("card"), "in", Prop(Zone)];

    [Optional]
    public TargetQualifier Qualifier { get; set; }
    public PlayersZone Zone { get; set; }
}
