namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A condition on what kind of card a revealed or named card is or isn't: "it's a creature card", "that card is a land card", "it's a creature or planeswalker card", "it isn't a land card", "it's a nonland card" (qualified kinds through QualifierWords), "that card is a creature card of the chosen type", or was, as it last existed: "it was a creature card".</summary>
/// <exampledoc>Call of the Wild</exampledoc>
/// <examplecapture>it's a creature card</examplecapture>
[Dependent]
public class CardIsKind : Glyph
{
    public override Nib[] Nibs => [Prop(Card), Pattern("an?"), Prop(Kind), "card", Prop(Chosen)];

    public OneOf<OneOf<ItIs, ThatCardIs, ItIsnt>, ItWas> Card { get; set; }
    public OneOf<ManyOf<PermanentKind>, PermanentKind, QualifierWords> Kind { get; set; }
    [Optional]
    public ChosenAspect? Chosen { get; set; }
}
