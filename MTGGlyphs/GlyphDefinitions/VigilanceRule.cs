namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Vigilance's reminder text.</summary>
/// <exampledoc>Brushstrider</exampledoc>
/// <examplecapture>attacking doesn't cause this creature to tap</examplecapture>
public class VigilanceRule : Glyph
{
    public override Nib[] Nibs => ["attacking doesn't cause this creature to tap"];
}
