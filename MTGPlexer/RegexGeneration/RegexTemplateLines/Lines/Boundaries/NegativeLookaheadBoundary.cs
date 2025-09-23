namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines.Boundaries;

/// <summary>
/// Placed at the end of composed regex patterns to ensure no matches end in the middle of words.
/// </summary>
public record NegativeLookaheadBoundary() 
    : BoundaryBase
    (
        Enclosures: [],
        Regex: @"(?!\w)",
        Comment: "boundary (don't end inside word)"
    );