namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>One rendered line of formatted output: a regex column followed by a colored, box-drawn comment, both broken into <see cref="SmartSpan"/>s.</summary>
public class SmartLine
{
    /// <summary>The ordered spans making up this line's text and coloring.</summary>
    public List<SmartSpan> Spans { get; }

    public SmartLine(List<SmartSpan> spans)
    {
        Spans = spans;
    }

    public override string ToString() => string.Join("", Spans);
}