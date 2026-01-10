namespace MTGPlexer.SnippetHelpers;

public record SnippetAlternatives : Snippet
{
    public SnippetAlternatives(params string[] alternatives) : base("(" + string.Join('|', alternatives) +")") { }
}
