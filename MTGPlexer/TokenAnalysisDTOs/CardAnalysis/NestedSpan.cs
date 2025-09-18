namespace MTGPlexer.TokenAnalysisDTOs.CardAnalysis;

public abstract record NestedSpan
(
    string Path,
    int NestedDepth,
    Palette Palette,
    bool IgnoreInAnalysis
);

