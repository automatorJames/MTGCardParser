namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Disguise's reminder text, first sentence.</summary>
/// <exampledoc>Forum Familiar</exampledoc>
/// <examplecapture>you may cast this card face down for {3} as a 2/2 creature with ward {2}</examplecapture>
public class DisguiseRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this card face down for {3} as a 2/2 creature with ward {2}"];
}
