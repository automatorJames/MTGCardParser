namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that card is": the card named earlier, as the subject of "is".</summary>
/// <exampledoc>Chaos Harlequin</exampledoc>
/// <examplecapture>that card is</examplecapture>
[Dependent]
public class ThatCardIs : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "is"];

    public ThatCardPhrase Card { get; set; }
}
