namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a dragon card from your hand": a card of a kind shown from a hand.</summary>
/// <exampledoc>Draconic Roar</exampledoc>
/// <examplecapture>a dragon card from your hand</examplecapture>
[Dependent]
public class RevealedFromHand : Glyph
{
    public override Nib[] Nibs => [Prop(Wanted), "from", Prop(Whose), "hand"];

    public WantedCards Wanted { get; set; }
    public Whose Whose { get; set; }
}
