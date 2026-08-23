namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>One rendered line of <see cref="GlyphClassRenderer"/>'s C# class output, broken into <see cref="ClassSpan"/>s - the class-view analog of <see cref="SmartLine"/>.</summary>
public class ClassLine
{
    /// <summary>The ordered spans making up this line's text and coloring.</summary>
    public List<ClassSpan> Spans { get; }

    public ClassLine(List<ClassSpan> spans)
    {
        Spans = spans;
    }

    public override string ToString() => string.Join("", Spans);
}
