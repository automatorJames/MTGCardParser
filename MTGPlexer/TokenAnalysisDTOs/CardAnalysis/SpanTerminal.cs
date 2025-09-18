namespace MTGPlexer.TokenAnalysisDTOs.CardAnalysis;

public abstract record SpanTerminal
(
    string Path,
    int NestedDepth,
    string Text,
    Palette Palette,
    bool IgnoreInAnalysis
) 
: NestedSpan(Path, NestedDepth, Palette, IgnoreInAnalysis);