using MTGPlexer.TokenAnalysis.ColorCoding;

namespace MTGPlexer.TokenAnalysis.MatchDTOs;

public abstract record SpanTerminal
(
    string Path,
    int NestedDepth,
    string Text,
    DeterministicPalette Palette,
    bool IgnoreInAnalysis
) 
: NestedSpan(Path, NestedDepth, Palette, IgnoreInAnalysis);