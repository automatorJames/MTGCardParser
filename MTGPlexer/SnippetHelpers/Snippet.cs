using System.Runtime.CompilerServices;

namespace MTGPlexer.SnippetHelpers;

public record Snippet
{
    public string Text { get; init; }
    public bool IsOptional { get; init; }
    public bool IsNoPrecedingSpace { get; init; }

    public Snippet()
    {
    }

    public Snippet(string text)
    {
        Text = text;
        IsOptional = this is SnippetOptional;
        IsNoPrecedingSpace = this is SnippetNoPrecedingSpace;
    }

    // Implicitly create a Snippet from a string
    public static implicit operator Snippet(string str) => new(str);


}