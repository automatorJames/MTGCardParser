namespace Glyphotype.GlyphAnalysisDTOs.WordTrees;

/// <summary>
/// The consolidated text of one <see cref="AdjacencyNode"/> plus the map of which top-level
/// <see cref="Glyph"/> type (if any) captured each stretch of it.
/// <para>
/// <paramref name="GlyphTypeNames"/> keys are character start indices within
/// <paramref name="Text"/>; each entry's value applies from that index forward until the next
/// entry's index. A null value marks the start of a stretch nothing captured (raw unmatched
/// words), which is what lets a run of glyph-captured text be terminated without an entry per
/// character. Null for the whole dictionary means the node is entirely uncaptured text.
/// </para>
/// </summary>
public record NodeSegment(string Text, Dictionary<int, string> GlyphTypeNames);
