namespace MTGPlexer.SnippetHelpers;

public record SnippetNoPrecedingSpace : Snippet
{
    public SnippetNoPrecedingSpace(string textWhichNoSpaceShouldPrecede) 
        : base(textWhichNoSpaceShouldPrecede) { }
}
