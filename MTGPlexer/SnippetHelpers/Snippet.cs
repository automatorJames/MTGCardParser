using System.Runtime.CompilerServices;

namespace MTGPlexer.SnippetHelpers;

public record Snippet
{
    public string Text { get; init; }
    public bool IsOptional { get; init; }

    public Snippet()
    {
    }

    public Snippet(string text, bool isOptional = false)
    {
        Text = text;
        IsOptional = isOptional;
    }

    // Implicitly create a Snippet from a string
    public static implicit operator Snippet(string str) => new(str);

    public static Snippet P(object member, [CallerArgumentExpression("member")] string expression = "")
    {
        // and not the prefix (this.), clean the string:
        var lastDot = expression.LastIndexOf('.');
        var name = lastDot == -1 ? expression : expression[(lastDot + 1)..];

        return new Snippet(name);
    }
}