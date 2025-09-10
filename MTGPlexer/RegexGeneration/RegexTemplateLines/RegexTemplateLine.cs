namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record RegexTemplateLine
(
    string Value, 
    string Path, 
    int Indentation,
    DeterministicPalette Palette = null,
    string CommentOne = null,
    string CommentTwo = null
)
{
    const int _spacesPerIndent = 4;

    public string IndentedValue { get; } = string.Empty.PadLeft(Indentation * _spacesPerIndent) + Value;

    public int Start { get; } = Indentation * _spacesPerIndent;
    public int End { get; } = Indentation * _spacesPerIndent + Value.Length;

    public int CommentOneLength { get; } = CommentOne?.Length ?? 0;
    public int CommentTwoLength { get; } = CommentTwo?.Length ?? 0;

}