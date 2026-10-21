namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Who searches a library, and whether they may choose not to: "you may", "target player", "its controller may".</summary>
/// <exampledoc>Ghost Quarter</exampledoc>
/// <examplecapture>its controller may</examplecapture>
[Dependent]
public class LibrarySearcher : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(May)];

    public Subject Player { get; set; }
    [Optional]
    [RegexPattern("may")]
    public bool? May { get; set; }
}
