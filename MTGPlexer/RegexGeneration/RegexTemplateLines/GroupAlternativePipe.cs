namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record GroupAlternativePipe(string Path, int Indentation, RegexPropInfo Group) 
    : RegexTemplateLine($"|", Path, Indentation, CommentOne: "alternate divider", Group: Group);