
namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickGroupClose : RegexBrickGroupBookend
{
    public RegexBrickGroupClose(RegexNode parentNode, GroupQuantifier? quantifier, string comment) 
        : base(parentNode, GetRegex(quantifier), comment)
    {
    }

    static string GetRegex(GroupQuantifier? quantifier) =>
        $"){quantifier?.GetDescription()}";
}

