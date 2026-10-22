namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"then shuffle" / "then shuffles": the shuffle that ends a library search.</summary>
/// <exampledoc>Borderland Ranger</exampledoc>
/// <examplecapture>then shuffle</examplecapture>
[Dependent]
public class ThenShuffle : Glyph
{
    public override Nib[] Nibs => ["then", Alt("shuffle", "shuffles")];
}
