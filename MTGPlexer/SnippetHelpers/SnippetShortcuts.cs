using System.Runtime.CompilerServices;

namespace MTGPlexer.SnippetHelpers;

public static class SnippetShortcuts
{
    public static Snippet Prop(object member, [CallerArgumentExpression("member")] string expression = "")
    {
        var lastDot = expression.LastIndexOf('.');
        var name = lastDot == -1 ? expression : expression[(lastDot + 1)..];

        return new Snippet(name);
    }

    public static SnippetAlternatives Alt(params string[] alternatives) =>
        new SnippetAlternatives(alternatives);

    public static SnippetOptional Opt(string optionalText) =>
        new SnippetOptional(optionalText);

    public static SnippetNoPrecedingSpace NoSpace(string text) =>
        new SnippetNoPrecedingSpace(text);
}
