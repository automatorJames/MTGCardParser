namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>A synthetic brick representing a blank separator line, inserted between logical sections of formatted output.</summary>
public class RegexBrickBlank : RegexBrick
{
    public RegexBrickBlank(RegexNode parentNode)
        : base(parentNode, null)
    {
    }
}