using System.ComponentModel;

namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record GroupClose(string Path, int Indentation, GroupQuantifier? Quantifier = null) 
    : RegexTemplateLine($"){(Quantifier.HasValue ?  Quantifier.Value.Description() : "")}", Path, Indentation);

public enum GroupQuantifier
{
    [Description("*")]
    AnyNumber,

    [Description("+")]
    OneOrMore,

    [Description("?")]
    Optional
}