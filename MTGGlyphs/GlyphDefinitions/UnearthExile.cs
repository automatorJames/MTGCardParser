namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Unearth's reminder text, third sentence: the unearthed card's exile clause.</summary>
/// <exampledoc>Rotting Rats</exampledoc>
/// <examplecapture>exile it at the beginning of the next end step or if it would leave the battlefield</examplecapture>
public class UnearthExile : Glyph
{
    public override Nib[] Nibs => ["exile it at the beginning of the next end step or if it would leave the battlefield"];
}
