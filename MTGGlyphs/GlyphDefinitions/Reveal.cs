namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Revealing cards: "target opponent reveals their hand", "reveal the top four cards of your library", "reveal a dragon card from your hand".</summary>
/// <exampledoc>Distress</exampledoc>
/// <examplecapture>target player reveals their hand</examplecapture>
public class Reveal : Glyph
{
    public override Nib[] Nibs => [Prop(Who), Pattern("reveals?"), Prop(What)];

    [Optional]
    public Subject Who { get; set; }
    public RevealedThing What { get; set; }
}
