namespace MTGPlexer.TokenAnalysis.DTOs;

/// <summary>
/// A part of a leaf that is simple, un-captured text.
/// </summary>
/// <param name="Text">The raw text to render.</param>
public record NonPropertyTokenLeafPart(string Text) : TokenLeafPart(Text);
