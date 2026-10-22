namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of your library", "of target player's library", "of their owners' libraries": whose library a position is in.</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>of your library</examplecapture>
[Dependent]
public class OfLibrary : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Owner), Pattern("librar(y|ies)")];

    public Owner Owner { get; set; }
}
