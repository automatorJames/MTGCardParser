namespace MTGCardParser;

public class Card
{
    public static string ThisToken = "{this}";

    public int CardId { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public string ManaCost { get; set; }
    public string Types { get; set; }
    public string Supertypes { get; set; }
    public string Subtypes { get; set; }
    public string Keywords { get; set; }
    public string Power { get; set; }
    public string Toughness { get; set; }
    public string Loyalty { get; set; }
    public string SetCode { get; set; }
    public int SetSequence { get; set; }

    string[] _cleanedLines; 
    public string[] CleanedLines
    {
        get
        {
            if (_cleanedLines is null)
                _cleanedLines = GetCleanedLines();

            return _cleanedLines;
        }
    }

    public List<List<Token<Type>>> ProcessedLineTokens { get; set; } = new();

    public List<Token<Type>> CombinedTokens => ProcessedLineTokens.SelectMany(x => x).ToList();

    string[] GetCleanedLines()
    {
        if (Text is null)
            return [];

        var text = Text;
        text = text.Replace(Name, ThisToken);
        text = text.ToLower();
        var lines = text.Split('\n');

        return lines;
    }
}

