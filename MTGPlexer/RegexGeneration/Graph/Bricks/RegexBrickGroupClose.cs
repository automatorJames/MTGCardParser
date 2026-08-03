
namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>The closing brick of a named group, e.g. <c>)</c> or <c>)*</c> when a quantifier applies.</summary>
public class RegexBrickGroupClose : RegexBrickGroupBookend
{
    public RegexBrickGroupClose(RegexNode parentNode, Quantifier? quantifier)
        : base(parentNode, GetRegex(quantifier))
    {
    }

    static string GetRegex(Quantifier? quantifier) =>
        $"){quantifier?.GetDescription()}";
}

