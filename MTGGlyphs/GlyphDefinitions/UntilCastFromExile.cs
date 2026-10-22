namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A duration ending when the card itself is cast from exile: "until this card is cast from exile".</summary>
/// <exampledoc>Rakish Revelers</exampledoc>
/// <examplecapture>until this card is cast from exile</examplecapture>
[Dependent]
public class UntilCastFromExile : Glyph
{
    public override Nib[] Nibs => ["until", Prop(Card), "is cast from exile"];

    public OneOf<This, ThisCreature> Card { get; set; }
}
