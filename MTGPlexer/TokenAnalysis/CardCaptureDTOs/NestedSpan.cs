namespace MTGPlexer.TokenAnalysis.CardCaptureDTOs;

public abstract record NestedSpan
(
    string Path,
    int NestedDepth,
    DeterministicPalette Palette,
    bool IgnoreInAnalysis
);

