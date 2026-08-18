namespace Glyphotype.GlyphAnalysisDTOs.WordTrees;

/// <summary>
/// A record to hold structured information about a single token (a word or a recognized token type).
/// </summary>
public record GlyphInfo(string Text, Type GlyphType);
