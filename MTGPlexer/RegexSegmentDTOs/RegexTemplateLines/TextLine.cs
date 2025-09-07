namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record TextLine(string Value, string Path, int Indentation) 
    : RegexTemplateLine(Value, Path, Indentation);
