namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What amplify counts: "zombie card you reveal in your hand" - cards of a kind revealed from hand as the permanent enters.</summary>
/// <exampledoc>Embalmed Brawler</exampledoc>
/// <examplecapture>zombie card you reveal in your hand</examplecapture>
[Dependent]
public class CardsRevealedFromHand : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "card you reveal in your hand"];

    public QualifierWords Kind { get; set; }
}
