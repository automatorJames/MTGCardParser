namespace Glyphotype.GlyphPrimitives;

[Dependent]
public class DynamicGlyph : Glyph
{
    public override Joiner Joiner => Joiner.Pipe;

    public object Item { get; }
    public Type ResolvedType { get; }

    public DynamicGlyph()
    {
    }

    public DynamicGlyph(object item)
    {
        Item = item;
        ResolvedType = item.GetType();
    }
}
