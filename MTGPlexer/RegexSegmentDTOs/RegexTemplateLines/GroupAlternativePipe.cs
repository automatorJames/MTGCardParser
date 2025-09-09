namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record GroupAlternativePipe(string Path, int Indentation) 
    : RegexTemplateLine($"|", Path, Indentation, CommentOne: "group alternate divider");