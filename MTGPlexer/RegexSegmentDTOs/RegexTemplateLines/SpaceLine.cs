namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record SpaceLine(string Path, int Indentation) 
    : RegexTemplateLine("[ ]", Path, Indentation, CommentOne: "connective space");