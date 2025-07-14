namespace MTGPlexer.TokenAnalysis.DTOs;

public abstract record NestedSpanTerminal
(
    string Path,
    int NestedDepth,
    string Text,
    DeterministicPalette Palette
) 
: NestedSpan(Path, NestedDepth, Palette);