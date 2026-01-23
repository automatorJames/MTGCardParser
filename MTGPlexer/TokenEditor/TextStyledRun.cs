namespace MTGPlexer.TokenEditor;

public record TextStyledRun(string Text, string Color, string UnderlineClass)
    : StyledRun(Text, Color);
