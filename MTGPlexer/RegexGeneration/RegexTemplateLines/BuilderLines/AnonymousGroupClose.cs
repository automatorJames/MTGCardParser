using System.ComponentModel;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class AnonymousGroupClose : EnclosureBookend, IGroupClose
{
    public AnonymousGroupClose(Enclosure[] enclosures, GroupQuantifier? quantifier = null)
        : base(
            enclosures,
            $"){(quantifier.HasValue ? quantifier.Value.GetDescription() : "")}",
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