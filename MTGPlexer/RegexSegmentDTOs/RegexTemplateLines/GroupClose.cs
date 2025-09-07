namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record GroupClose(string Path, int Indentation, bool GroupIsOptional = false) 
    : RegexTemplateLine($"){(GroupIsOptional ? "?" : "")}", Path, Indentation);