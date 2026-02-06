namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Boundaries;

/// <summary>
/// Placed at the end of composed regex patterns to ensure no matches end in the middle of words.
/// </summary>
public class StartOfLineBoundary : AtomElement
{
    public StartOfLineBoundary()
        : base([], @"^", "boundary (start of line)")
    {
    }
}