namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder text defining a crime.</summary>
/// <exampledoc>Deepmuck Desperado</exampledoc>
/// <examplecapture>targeting opponents, anything they control, and/or cards in their graveyards is a crime</examplecapture>
public class CrimeRule : Glyph
{
    public override Nib[] Nibs => ["targeting opponents, anything they control, and/or cards in their graveyards is a crime"];
}
