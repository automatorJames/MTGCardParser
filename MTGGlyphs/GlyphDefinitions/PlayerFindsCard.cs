namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The stop condition of a reveal-until loop, said of the player: "you reveal a land card", "they reveal a nonland card", "they reveal two land cards".</summary>
/// <exampledoc>Balustrade Spy</exampledoc>
/// <examplecapture>they reveal a land card</examplecapture>
[Dependent]
public class PlayerFindsCard : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Verb), Prop(Card)];

    public OneOf<WhichPlayer?, TheyPlayer> Player { get; set; }
    public RevealOrExile Verb { get; set; }
    public OneOf<AnyCard, CountedCards> Card { get; set; }
}
