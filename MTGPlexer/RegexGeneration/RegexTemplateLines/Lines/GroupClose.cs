using System.ComponentModel;

namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record GroupClose
(
    Enclosure[] Enclosures,
    Palette Palette, 
    GroupQuantifier? Quantifier = null
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: $"){(Quantifier.HasValue ?  Quantifier.Value.Description() : "")}",
        Palette: Palette, 
        Comment: GetComment(Quantifier)
    )
{
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