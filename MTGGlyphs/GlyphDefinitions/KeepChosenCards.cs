namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player keeping some cards from a zone and losing the rest: "target player chooses a card in their hand and discards the rest", "each opponent chooses two cards in their graveyard and exiles the rest", "choose x cards in your hand and discard the rest", "… and shuffles the rest into their library".</summary>
/// <exampledoc>Monomania</exampledoc>
/// <examplecapture>target player chooses a card in their hand and discards the rest</examplecapture>
public class KeepChosenCards : Glyph
{
    public override Nib[] Nibs => [Prop(Chooser), Alt("choose", "chooses"), Prop(Kept), "and", Prop(Rest)];

    [Optional]
    public Subject Chooser { get; set; }
    public CountedCardsInZone Kept { get; set; }
    public OneOf<RestRemoved, ShuffleRestIntoLibrary> Rest { get; set; }
}
