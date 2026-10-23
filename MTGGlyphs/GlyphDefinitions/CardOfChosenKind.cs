namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card picked out by a choice made earlier in the ability: "a card with the chosen name", "a card of the chosen type", "a card of the chosen color". Used where an effect then checks what it found ("if a card with the chosen name was milled this way, …").</summary>
/// <exampledoc>Predict</exampledoc>
/// <examplecapture>a card with the chosen name</examplecapture>
[Dependent]
public class CardOfChosenKind : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), "card", Prop(Chosen)];

    public OneOf<WithChosenName, ChosenAspect?> Chosen { get; set; }
}
