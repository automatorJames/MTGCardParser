using System.ComponentModel;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class GroupClose : EncloureBookend, IGroupClose
{
    public GroupClose(Enclosure[] enclosures, GroupQuantifier? quantifier = null)
        : base(
            enclosures,
            $"){(quantifier.HasValue ? quantifier.Value.Description() : "")}",
            GetComment(quantifier)
        )
    {
    }

    static string GetComment(GroupQuantifier? quantifier)
    {
        if (quantifier == null)
            return null;

        return quantifier.Value.ToString().ToFriendlyCase();
    }

    public override string ToString() => base.ToString();
}

public enum GroupQuantifier
{
    [Description("*")]
    AnyNumber,

    [Description("+")]
    OneOrMore,

    [Description("?")]
    Optional
}