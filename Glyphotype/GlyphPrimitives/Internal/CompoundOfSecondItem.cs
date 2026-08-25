namespace Glyphotype.GlyphPrimitives.Internal;

public class CompoundOfSecondItem<T> : Glyph
{
    //public override Joiner Joiner => Joiner.None;
    public override Joiner Joiner => Joiner.CommaSpace;

    public T Item { get; set; }
}
