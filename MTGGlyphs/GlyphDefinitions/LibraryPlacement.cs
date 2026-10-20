namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A position in a library: "on the bottom of your library in any order", "on top of their library", "on the bottom in a random order".</summary>
/// <exampledoc>Impulse</exampledoc>
/// <examplecapture>on the bottom of your library in any order</examplecapture>
[Dependent]
public class LibraryPlacement : Glyph
{
    public override Nib[] Nibs => ["on", Prop(End), Prop(Library), Prop(Order)];

    public LibraryEnd End { get; set; }
    [Optional]
    public OfLibrary Library { get; set; }
    [Optional]
    public LibraryOrder? Order { get; set; }
}
