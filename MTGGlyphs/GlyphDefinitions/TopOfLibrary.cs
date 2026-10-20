namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the top of your library": the top of a player's library as a place cards are played or put.</summary>
/// <exampledoc>Courser of Kruphix</exampledoc>
/// <examplecapture>the top of your library</examplecapture>
[Dependent]
public class TopOfLibrary : Glyph
{
    public override Nib[] Nibs => ["the top of", Prop(Owner), "library"];

    public Owner Owner { get; set; }
}
