
namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>The closing brick of a named group, e.g. <c>)</c> or <c>)*</c> when a quantifier applies.</summary>
public class RegexBrickGroupClose : RegexBrickGroupBookend
{
    /// <summary>Display-only: the group's own name, shown on its closing border. Populated by <c>BrickCommentResolver</c>.</summary>
    public string NameText { get; set; } = "";

    /// <summary>Display-only: the "(quantifier)" parenthetical appended when the group carries a non-default quantifier, or "" when not needed. Populated by <c>BrickCommentResolver</c>.</summary>
    public string QuantifierReminder { get; set; } = "";

    public RegexBrickGroupClose(RegexNode parentNode, Quantifier? quantifier)
        : base(parentNode, GetRegex(quantifier))
    {
    }

    static string GetRegex(Quantifier? quantifier) =>
        $"){quantifier?.GetDescription()}";
}

