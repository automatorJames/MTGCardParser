namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Delve's reminder text.</summary>
/// <exampledoc>Treasure Cruise</exampledoc>
/// <examplecapture>each card you exile from your graveyard while casting this spell pays for {1}</examplecapture>
public class DelveRule : Glyph
{
    public override Nib[] Nibs => ["each card you exile from your graveyard while casting this spell pays for {1}"];
}
