namespace MTGPlexer.Interfaces;

public interface IDocument
{
    public static string ThisToken = "{this}";

    public string Name { get; }
    public string Text { get; }

    public string[] GetFormattedLines()
    {
        if (Text is null)
            return [];

        var text = Text.ToLower();
        text = text.Replace(Name, ThisToken);
        var lines = text.Split('\n').Select(x => x.Trim()).ToArray();

        return lines;
    }
}
