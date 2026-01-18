namespace MTGPlexer.SnippetHelpers;

public record SnippetOptionalPlural : Snippet
{
    public SnippetOptionalPlural() 
        : base("(s|es|ies)?") { }
}
