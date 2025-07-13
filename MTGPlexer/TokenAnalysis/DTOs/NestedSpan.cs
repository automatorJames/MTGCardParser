namespace MTGPlexer.TokenAnalysis.DTOs;

public abstract record NestedSpan
(
    string Path,
    int NestedDepth,
    DeterministicPalette Palette
);

