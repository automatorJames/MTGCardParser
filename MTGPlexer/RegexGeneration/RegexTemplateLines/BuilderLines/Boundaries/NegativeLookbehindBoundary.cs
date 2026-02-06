namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Boundaries;

/// <summary>
/// Placed at the start of composed regex patterns to ensure no matches begin in the middle of words.
/// </summary>
public class NegativeLookbehindBoundary : AtomElement
{
    public NegativeLookbehindBoundary()
        : base([], @"(?<!\w)", "boundary (don't start inside word)")
    {
    }
}