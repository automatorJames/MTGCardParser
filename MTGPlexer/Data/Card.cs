namespace MTGPlexer.Data;

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

    string[] _formattedLines;
    public string[] FormattedLines
    {
        get
        {
            if (_formattedLines is null)
                _formattedLines = GetFormattedLines();

            return _formattedLines;
        }
    }

    string[] _formattedLinesLower;
    public string[] FormattedLinesLower
    {
        get
        {
            if (_formattedLinesLower is null)
                _formattedLinesLower = FormattedLines.Select(x => x.ToLower()).ToArray();

            return _formattedLinesLower;
        }
    }

    string[] GetFormattedLines()
    {
        if (Text is null)
            return [];

        var text = Text;
        text = text.Replace(Name, ThisToken);
        var lines = text.Split('\n').Select(x => x.Trim()).ToArray();

        return lines;
    }


    public override string ToString() => Name;
}

