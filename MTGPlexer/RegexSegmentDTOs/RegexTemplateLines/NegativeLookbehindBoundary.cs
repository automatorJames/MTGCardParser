namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

/// <summary>
/// Placed at the start of composed regex patterns to ensure no matches begin in the middle of words.
/// </summary>
public record NegativeLookbehindBoundary() 
    : RegexTemplateLine(@"(?<!\w)", string.Empty, 0, CommentOne: "boundary (don't start inside word)");