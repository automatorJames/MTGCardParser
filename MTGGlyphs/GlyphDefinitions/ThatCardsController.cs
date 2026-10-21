namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that land's controller", "its controller", "its owner", "this creature's owner": the controller or owner of a permanent named earlier in the line.</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>that land's controller</examplecapture>
[Dependent]
public class ThatCardsController : Glyph
{
    public override Nib[] Nibs => [Prop(Card), Prop(Role)];

    public OneOf<Its, ObjectPossessive> Card { get; set; }
    public PlayerRole Role { get; set; }
}
