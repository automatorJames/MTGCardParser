namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record SpaceLine(string Path, int Indentation) 
    : RegexTemplateLine("[ ]", Path, Indentation, CommentOne: "connective space");