namespace MTGCardParser;

public class TokenSegment(string text)
{
    public string Text { get; set; } = text;

    public override string ToString() => Text;
}

