namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Storm's reminder text.</summary>
/// <exampledoc>Brain Freeze</exampledoc>
/// <examplecapture>when you cast this spell, copy it for each spell cast before it this turn</examplecapture>
public class StormRule : Glyph
{
    public override Nib[] Nibs => ["when you cast this spell, copy it for each spell cast before it this turn"];
}
