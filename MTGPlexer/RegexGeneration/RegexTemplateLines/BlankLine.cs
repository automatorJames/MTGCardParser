namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record BlankLine(string Path) 
    : RegexTemplateLine("", Path, 0);