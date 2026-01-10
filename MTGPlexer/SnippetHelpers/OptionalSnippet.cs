namespace MTGPlexer.SnippetHelpers;

public record OptionalSnippet : Snippet
{
    public OptionalSnippet(string text) : base(text, isOptional: true) { }
}
