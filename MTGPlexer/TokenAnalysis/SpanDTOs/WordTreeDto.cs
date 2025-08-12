namespace MTGPlexer.TokenAnalysis.SpanDTOs;

/// <summary>
/// A data transfer object representing the entire span visualization for JS.
/// </summary>
public record WordTreeDto(
    string Id,
    string Text,
    List<WordTreeNodeDto> Preceding,
    List<WordTreeNodeDto> Following,
    List<List<string>> Sentences);
