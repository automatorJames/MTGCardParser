namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record TextLine(string Value, string Path, int Indentation) 
    : RegexTemplateLine(Value.Replace(" ", "[ ]"), Path, Indentation, CommentOne: "literal match");
