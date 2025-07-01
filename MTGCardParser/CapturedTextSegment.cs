namespace MTGCardParser;

public class CapturedTextSegment(string text)
{
    public string Text { get; set; } = text;

    public override string ToString() => Text;
}

