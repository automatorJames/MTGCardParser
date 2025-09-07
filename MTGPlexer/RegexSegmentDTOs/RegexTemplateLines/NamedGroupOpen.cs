namespace MTGPlexer.RegexSegmentDTOs.RegexTemplateLines;

public record NamedGroupOpen(string Name, string Path, int Indentation) 
    : RegexTemplateLine($"(?<{Name}>", Path, Indentation);