namespace Glyphotype.GlyphAnalysisDTOs.WordTrees;

/// <summary>
/// One fully-maximal corpus span ("echo") found within a specific <see cref="UnmatchedTextOccurrence"/>,
/// at a specific word position, along with the total number of times it occurs across the whole
/// corpus, this document's own occurrences included. <see cref="DisplayCount"/> is never shown
/// below 2 — <see cref="DigestedText.FindEchoes"/> only surfaces a span at all once it's confirmed
/// to occur at least once outside this document, so a span reaching here already has at least one
/// occurrence here and at least one elsewhere.
/// </summary>
public record EchoMatch(AnalyzedText Span, int WordStartIndex, int DisplayCount);
