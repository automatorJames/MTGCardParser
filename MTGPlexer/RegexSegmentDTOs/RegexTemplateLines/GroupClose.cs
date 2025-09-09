using System.ComponentModel;

namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record GroupClose(string Path, int Indentation, DeterministicPalette Palette, string Name, GroupQuantifier? Quantifier = null) 
    : RegexTemplateLine
    (
        $"){(Quantifier.HasValue ?  Quantifier.Value.Description() : "")}", 
        Path, 
        Indentation, 
        Palette, 
        CommentTwo: Name
    );

public enum GroupQuantifier
{
    [Description("*")]
    AnyNumber,

    [Description("+")]
    OneOrMore,

    [Description("?")]
    Optional
}