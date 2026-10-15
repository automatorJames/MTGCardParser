namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Madness's first reminder sentence.</summary>
/// <exampledoc>Abandon Reason</exampledoc>
/// <examplecapture>if you discard this card, discard it into exile</examplecapture>
public class MadnessExile : Glyph
{
    public override Nib[] Nibs => ["if you discard this card, discard it into exile"];
}
