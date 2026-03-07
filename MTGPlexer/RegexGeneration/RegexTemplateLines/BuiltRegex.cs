namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class BuiltRegex
{
    public string MinifiedRegex { get; }
    public string FormattedRegex { get; }
    public List<string> FormattedLines { get; }
    public Regex Regex { get; }

    public BuiltRegex(List<RegexBrick> regexBricks)
    {
        MinifiedRegex = string.Join("", regexBricks.Select(x => x.Regex)).Replace("[ ]", " ");
        FormattedLines = regexBricks.Where(x => !x.Parent.MayIgnoreInFormattedOutput).Select(x => new string(' ', x.NestedDepthFormatted * 4) + x.Regex).ToList();
        FormattedRegex = string.Join(Environment.NewLine, FormattedLines);
        Regex = new(MinifiedRegex, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }

    public override string ToString() => MinifiedRegex;
}
