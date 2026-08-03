namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>A single contiguous run of text within a <see cref="SmartLine"/>, colored with one palette and tagged with the graph node it renders.</summary>
public record SmartSpan
{
    /// <summary>The literal text this span renders.</summary>
    public string Content { get; init; }

    /// <summary>The fully qualified name of the graph node this span represents, used to correlate rendered text back to the graph.</summary>
    public string FullyQualifiedName { get; }

    /// <summary>The color palette this span is rendered with.</summary>
    public HexPalette Palette { get; }

    public SmartSpan(string content, string fullyQualifiedName, HexPalette hexPalette)
    {
        Content = content;
        FullyQualifiedName = fullyQualifiedName;
        Palette = hexPalette;
    }

    /// <summary>Returns a copy of this span with its text reversed, used to build the mirrored right-hand wall of a group box from its left-hand wall.</summary>
    public SmartSpan Mirror() =>
        this with { Content = string.Concat(Content.Reverse()) };

    public override string ToString() => Content;
}