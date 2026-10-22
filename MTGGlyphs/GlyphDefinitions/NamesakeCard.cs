namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Any one card in a player's zone a name is compared with: "a card in your graveyard", "another card in their hand".</summary>
/// <exampledoc>Frostpyre Arcanist</exampledoc>
/// <examplecapture>a card in your graveyard</examplecapture>
[Dependent]
public class NamesakeCard : Glyph
{
    public override Nib[] Nibs => [Prop(Determiner), "card in", Prop(Zone)];

    public IndefiniteDeterminer Determiner { get; set; }
    public PlayersZone Zone { get; set; }
}
