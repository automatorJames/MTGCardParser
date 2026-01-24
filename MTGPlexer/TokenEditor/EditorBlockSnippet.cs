namespace MTGPlexer.TokenEditor;

public abstract record EditorBlockSnippet : EditorSnippet
{
    protected EditorBlockSnippet(string editorRepresentation, string parameterRepresentation, string id)
        : base(editorRepresentation, parameterRepresentation, id)
    {
    }
}