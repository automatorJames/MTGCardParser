namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public record RegexTemplateLine
(
    string EvaluableRegex, 
    string Path, 
    int Indentation,
    Palette Palette = null,
    string CommentOne = null,
    string CommentTwo = null,
    RegexPropInfo Group = null
)
{
    const int _spacesPerIndent = 4;

    public string IndentedValue { get; } = string.Empty.PadLeft(Indentation * _spacesPerIndent) + EvaluableRegex;

    public int Start { get; } = Indentation * _spacesPerIndent;
    public int End { get; } = Indentation * _spacesPerIndent + EvaluableRegex.Length;

    public int CommentOneLength { get; } = CommentOne?.Length ?? 0;
    public int CommentTwoLength { get; } = CommentTwo?.Length ?? 0;

}