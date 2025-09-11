namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record GroupOpen(string Path, int Indentation, RegexPropInfo Group) 
    : RegexTemplateLine($"(", Path, Indentation, Group: Group);