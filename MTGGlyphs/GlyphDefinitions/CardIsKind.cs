namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on what kind of card a revealed or named card is or isn't: "it's a creature card", "that card is a land card", "it's a creature or planeswalker card", "it isn't a land card".</summary>
/// <exampledoc>Call of the Wild</exampledoc>
/// <examplecapture>it's a creature card</examplecapture>
[Dependent]
public class CardIsKind : Glyph
{
    public override Nib[] Nibs => [Prop(Card), Pattern("an?"), Prop(Kind), "card"];

    public OneOf<ItIs, ThatCardIs, ItIsnt> Card { get; set; }
    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
}
