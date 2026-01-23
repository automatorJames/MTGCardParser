namespace MTGPlexer.TokenEditor;

public record TextStyledRun(string Text, string Color = null, string UnderlineClass = null)
    : StyledRun(Text, Color);
