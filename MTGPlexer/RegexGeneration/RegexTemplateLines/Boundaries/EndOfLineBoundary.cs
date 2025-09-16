namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Boundaries;

/// <summary>
/// Placed at the end of composed regex patterns to ensure no matches end in the middle of words.
/// </summary>
public record EndOfLineBoundary() 
    : RegexTemplateLine(@"$", string.Empty, 0, CommentOne: "boundary (end of line)");