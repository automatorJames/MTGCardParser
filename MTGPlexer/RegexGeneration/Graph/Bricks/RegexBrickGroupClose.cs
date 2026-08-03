
namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickGroupClose : RegexBrickGroupBookend
{
    public RegexBrickGroupClose(RegexNode parentNode, Quantifier? quantifier, string comment) 
        : base(parentNode, GetRegex(quantifier), comment)
    {
    }

    static string GetRegex(Quantifier? quantifier) =>
        $"){quantifier?.GetDescription()}";
}

