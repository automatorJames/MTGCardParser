namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards of a kind the player owns outside the game (a Wish's sideboard): "a sorcery card you own from outside the game", "a card you own from outside the game", "up to four cards you own from outside the game".</summary>
/// <exampledoc>Burning Wish</exampledoc>
/// <examplecapture>a sorcery card you own from outside the game</examplecapture>
[Dependent]
public class CardsFromOutsideGame : Glyph
{
    public override Nib[] Nibs => [Prop(Wanted), "you own from outside the game"];

    public WantedCards Wanted { get; set; }
}
