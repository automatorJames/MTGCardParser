namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

/// <summary>
/// Placed at the end of composed regex patterns to ensure no matches end in the middle of words.
/// </summary>
public record NegativeLookaheadBoundary() 
    : RegexTemplateLine(@"(?!\w)", string.Empty, 0, CommentOne: "boundary (don't end inside word)");