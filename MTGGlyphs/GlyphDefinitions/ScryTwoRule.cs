namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Scry 2's reminder text.</summary>
/// <exampledoc>Horizon Scholar</exampledoc>
/// <examplecapture>look at the top two cards of your library, then put any number of them on the bottom of your library and the rest on top in any order</examplecapture>
public class ScryTwoRule : Glyph
{
    public override Nib[] Nibs => ["look at the top two cards of your library, then put any number of them on the bottom of your library and the rest on top in any order"];
}
