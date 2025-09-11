namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record SpaceLine(string Path, int Indentation, RegexPropInfo Group) 
    : RegexTemplateLine("[ ]", Path, Indentation, CommentOne: "connective space", Group: Group);