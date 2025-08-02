using MTGPlexer.TokenAnalysis.ColorCoding;

namespace MTGPlexer.TokenAnalysis.MatchDTOs;

public abstract record NestedSpan
(
    string Path,
    int NestedDepth,
    DeterministicPalette Palette,
    bool IgnoreInAnalysis
);

