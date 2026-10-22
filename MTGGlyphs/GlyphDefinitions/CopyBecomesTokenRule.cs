namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The rule that a copy of a permanent spell becomes a token as it resolves: "the copy becomes a token", reminder text after "copy that spell".</summary>
/// <exampledoc>Lithoform Engine</exampledoc>
/// <examplecapture>the copy becomes a token</examplecapture>
public class CopyBecomesTokenRule : Glyph
{
    public override Nib[] Nibs => ["the copy becomes a token"];
}
