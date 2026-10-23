namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of cards with something in common: "two cards that share a color", "two nonland cards that share a card type" - what a mill or discard turned up.</summary>
/// <exampledoc>Grindstone</exampledoc>
/// <examplecapture>two cards that share a color</examplecapture>
[Dependent]
public class CardsSharingAspect : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Prop(Qualifiers), "cards", Prop(Sharing)];

    public Quantity Count { get; set; }
    [Optional]
    public QualifierRun Qualifiers { get; set; }
    public ShareAnAspect Sharing { get; set; }
}
