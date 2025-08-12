namespace MTGPlexer.TokenAnalysis.SpanDTOs;

/// <summary>
/// A data transfer object representing a single node in the word tree for JS visualization.
/// </summary>
public record WordTreeNodeDto(
    string Id,
    string Text,
    string? TokenTypeColor,
    List<WordTreeNodeDto> Children);