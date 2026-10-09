namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that land's controller": the controller of a permanent named earlier in the line.</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>that land's controller</examplecapture>
[Dependent]
public class ThatCardsController : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "'s controller"];

    public ThatCard Card { get; set; }
}
