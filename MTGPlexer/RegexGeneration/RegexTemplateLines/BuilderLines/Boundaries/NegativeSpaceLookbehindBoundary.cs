namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Boundaries;

/// <summary>
/// Placed at the start of composed regex patterns to ensure no matches begin in the middle of words.
/// </summary>
public class NegativeSpaceLookbehindBoundary : AtomElement
{
    public NegativeSpaceLookbehindBoundary(Enclosure[] enclosures)
        : base(enclosures, @"(?<! )", "boundary (don't include trailing space)")
    {
    }
}