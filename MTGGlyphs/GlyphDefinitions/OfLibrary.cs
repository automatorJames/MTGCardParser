namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of your library", "of target player's library": whose library a position is in.</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>of your library</examplecapture>
[Dependent]
public class OfLibrary : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Owner), "library"];

    public Owner Owner { get; set; }
}
