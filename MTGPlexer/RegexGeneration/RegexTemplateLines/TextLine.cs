namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record TextLine(string Value, string Path, int Indentation, RegexPropInfo Group) 
    : RegexTemplateLine(Value.Replace(" ", "[ ]"), Path, Indentation, CommentOne: "literal match", Group: Group);
