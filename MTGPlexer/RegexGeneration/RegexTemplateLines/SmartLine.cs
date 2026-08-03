namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

public class SmartLine
{
    public List<SmartSpan> Spans { get; }

    public SmartLine(List<SmartSpan> spans)
    {
        Spans = spans;
    }

    public override string ToString() => string.Join("", Spans);
}