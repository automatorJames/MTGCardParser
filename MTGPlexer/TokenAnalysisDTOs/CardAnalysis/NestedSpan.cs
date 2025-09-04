namespace MTGPlexer.TokenAnalysisDTOs.CardAnalysis;

public abstract record NestedSpan
(
    string Path,
    int NestedDepth,
    DeterministicPalette Palette,
    bool IgnoreInAnalysis
);

