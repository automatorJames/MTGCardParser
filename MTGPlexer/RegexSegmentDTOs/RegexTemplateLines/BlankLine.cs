namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record BlankLine(string Path) 
    : RegexTemplateLine("", Path, 0);