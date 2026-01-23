namespace MTGPlexer.TokenEditor;

public record RegexStyledRun(string Text, string Color)
    : StyledRun(Text, Color);
