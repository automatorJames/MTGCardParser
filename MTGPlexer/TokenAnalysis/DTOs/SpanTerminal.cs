namespace MTGPlexer.TokenAnalysis.DTOs;

public abstract record SpanTerminal
(
    string Path,
    int NestedDepth,
    string Text,
    DeterministicPalette Palette,
    bool IgnoreInAnalysis
) 
: NestedSpan(Path, NestedDepth, Palette, IgnoreInAnalysis);