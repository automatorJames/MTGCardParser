namespace Glyphotype.GlyphPrimitives.Internal;

public class CompoundOfSecondItem<T> : Glyph
{
    public override Joiner Joiner => Joiner.None;

    public T Item { get; set; }
}
