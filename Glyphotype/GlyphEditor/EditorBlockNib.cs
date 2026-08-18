namespace Glyphotype.GlyphEditor;

public abstract record EditorBlockNib : EditorNib
{
    protected EditorBlockNib(string editorRepresentation, string parameterRepresentation, string id)
        : base(editorRepresentation, parameterRepresentation, id)
    {
    }
}