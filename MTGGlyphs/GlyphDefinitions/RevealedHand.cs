namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"their hand", "your hand": a whole hand revealed.</summary>
/// <exampledoc>Distress</exampledoc>
/// <examplecapture>their hand</examplecapture>
[Dependent]
public class RevealedHand : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), "hand"];

    public Whose Whose { get; set; }
}
