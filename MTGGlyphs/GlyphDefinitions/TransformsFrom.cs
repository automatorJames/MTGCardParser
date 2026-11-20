namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A back face's reminder of the front face it transforms from, a line of its own: "(transforms from azor's gateway". The front face's name is open text.</summary>
/// <exampledoc>Azor's Gateway // Sanctum of the Sun</exampledoc>
/// <examplecapture>(transforms from azor's gateway</examplecapture>
public class TransformsFrom : Glyph
{
    public override Nib[] Nibs => ["(transforms from", Pattern("[a-z][a-z' ,-]*")];
}
