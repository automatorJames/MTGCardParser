namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Phased-out permanents' reminder text.</summary>
/// <exampledoc>Mist Dragon</exampledoc>
/// <examplecapture>while it's phased out, it's treated as though it doesn't exist</examplecapture>
public class PhasedOutRule : Glyph
{
    public override Nib[] Nibs => ["while it's phased out, it's treated as though it doesn't exist"];
}
