namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines.Boundaries;

/// <summary>
/// Placed at the end of composed regex patterns to ensure no matches end in the middle of words.
/// </summary>
public record EndOfLineBoundary() 
    : BoundaryBase
    (
        Enclosures: [],
        Regex: @"$", 
        Comment: "boundary (end of line)"
    );