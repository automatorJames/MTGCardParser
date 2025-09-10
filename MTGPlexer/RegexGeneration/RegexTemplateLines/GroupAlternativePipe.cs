namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record GroupAlternativePipe(string Path, int Indentation) 
    : RegexTemplateLine($"|", Path, Indentation, CommentOne: "alternate divider");