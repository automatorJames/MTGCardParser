namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines.Boundaries;

/// <summary>
/// Placed at the start of composed regex patterns to ensure no matches begin in the middle of words.
/// </summary>
public class NegativeLookbehindBoundary : BoundaryBase
{
    public NegativeLookbehindBoundary()
        : base([], @"(?<!\w)", "boundary (don't start inside word)")
    {
    }
}