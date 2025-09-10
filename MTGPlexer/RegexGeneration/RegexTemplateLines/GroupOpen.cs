namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record GroupOpen(string Path, int Indentation) 
    : RegexTemplateLine($"(", Path, Indentation);