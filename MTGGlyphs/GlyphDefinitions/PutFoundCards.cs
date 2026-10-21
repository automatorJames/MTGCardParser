namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where the cards found by a search go: "put it into your hand", "puts it into their hand", "put those cards onto the battlefield tapped", "put that card into your graveyard", "put it onto the battlefield under your control".</summary>
/// <exampledoc>Borderland Ranger</exampledoc>
/// <examplecapture>put it into your hand</examplecapture>
[Dependent]
public class PutFoundCards : Glyph
{
    public override Nib[] Nibs => [Alt("put", "puts"), Alt("it", "that card", "them", "those cards"), Prop(Destination)];

    public OneOf<FoundCardsZone?, ToTheBattlefieldUnderControl> Destination { get; set; }
}
