namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the copy", "the copies": copies of a spell or card made earlier, to be cast.</summary>
/// <exampledoc>Isochron Scepter</exampledoc>
/// <examplecapture>the copy</examplecapture>
[Dependent]
public class TheCopy : Glyph
{
    public override Nib[] Nibs => ["the", Alt("copy", "copies")];
}
