namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's library being empty: "your library has no cards in it".</summary>
/// <exampledoc>Laboratory Maniac</exampledoc>
/// <examplecapture>your library has no cards in it</examplecapture>
[Dependent]
public class LibraryHasNoCards : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), "library has no cards in it"];

    public Whose Whose { get; set; }
}
