namespace Glyphotype.GlyphAnalysisDTOs.TypeExpressions;

/// <summary>
/// One matched instance of a <see cref="GlyphOccurrenceSummary"/>'s top-level <see cref="Glyph"/> type:
/// which document it came from, and the hydrated <see cref="Glyph"/> itself - <see cref="CaptureTrace"/>
/// exposes its source <see cref="Glyphotype.RegexGeneration.Graph.CaptureContext.RootCaptureTrace"/> for
/// TypeRegexPage's "Matches" footer tray view (see <see cref="Glyphotype.RegexGeneration.Presentation.MatchContentRenderer"/>).
/// </summary>
public record MatchOccurrence(string DocumentName, Glyph Glyph)
{
    public RootCaptureTrace CaptureTrace => Glyph.CaptureContext.RootCaptureTrace;
}
