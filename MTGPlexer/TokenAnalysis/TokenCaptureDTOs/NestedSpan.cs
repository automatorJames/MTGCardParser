namespace MTGPlexer.TokenAnalysis.TokenCaptureDTOs;

public abstract record NestedSpan
(
    string Path,
    int NestedDepth,
    DeterministicPalette Palette,
    bool IgnoreInAnalysis
);

