namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines.Boundaries;

/// <summary>
/// Placed at the start of composed regex patterns to ensure no matches begin in the middle of words.
/// </summary>
public record NegativeLookbehindBoundary() 
    : BoundaryBase
    (
        Enclosures: [],
        Regex: @"(?<!\w)",
        Comment: "boundary (don't start inside word)"
    );