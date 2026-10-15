namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cascade's reminder text, tail.</summary>
/// <exampledoc>Ardent Plea</exampledoc>
/// <examplecapture>you may cast it without paying its mana cost. put the exiled cards on the bottom of your library in a random order</examplecapture>
public class CascadeTail : Glyph
{
    public override Nib[] Nibs => ["you may cast it without paying its mana cost. put the exiled cards on the bottom of your library in a random order"];
}
