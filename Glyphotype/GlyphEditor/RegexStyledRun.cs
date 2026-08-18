namespace Glyphotype.GlyphEditor;

public record RegexStyledRun(string Text, string Color)
    : StyledRun(Text, Color);
