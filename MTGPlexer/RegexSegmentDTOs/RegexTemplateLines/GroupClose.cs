using System.ComponentModel;

namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record GroupClose(string Path, int Indentation, DeterministicPalette Palette, string Name, GroupQuantifier? Quantifier = null) 
    : RegexTemplateLine
    (
        $"){(Quantifier.HasValue ?  Quantifier.Value.Description() : "")}", 
        Path, 
        Indentation, 
        Palette, 
        GetCommentOne(Quantifier),
        GetCommentTwo(Name)
    )
{
    static string GetCommentOne(GroupQuantifier? quantifier)
    {
        if (quantifier == null)
            return null;

        return quantifier.Value.ToString().ToFriendlyCase();
    }

    static string GetCommentTwo(string name)
    {
        if (name == null)
            return null;

        return name.ToFriendlyCase(TitleDisplayOption.Title);
    }

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