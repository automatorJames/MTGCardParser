using System.Runtime.CompilerServices;

namespace MTGPlexer.SnippetHelpers;

public static class SnippetShortcuts
{
    public static Snippet Prop(object member, Proptions proptions = Proptions.None, [CallerArgumentExpression("member")] string expression = "")
    {
        var lastDot = expression.LastIndexOf('.');
        var name = lastDot == -1 ? expression : expression[(lastDot + 1)..];

        return new Snippet(name, proptions)
        {
            IsPlural = proptions.HasFlag(Proptions.Plural),
            IsOptional = proptions.HasFlag(Proptions.Optional),
        };
    }

    public static SnippetAlternatives Alt(params string[] alternatives) =>
        new SnippetAlternatives(alternatives);

    public static SnippetOptional Opt(string optionalText) =>
        new SnippetOptional(optionalText);

    public static SnippetNoPrecedingSpace NoSpace(string text) =>
        new SnippetNoPrecedingSpace(text);

    public static SnippetOptionalPlural Plural() =>
        new SnippetOptionalPlural();

    public static IReadOnlyList<string> GetPublicStaticMethodNames()
    {
        return typeof(SnippetShortcuts)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .ToList();
    }
}

[Flags]
public enum Proptions
{
    None,
    Plural,
    Optional,
    NoPrecedingSpace,
}