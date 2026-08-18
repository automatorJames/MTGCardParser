namespace Glyphotype.GlyphPrimitives.Internal;

public class CompoundOfSecondItem<T> : Glyph
{
    public override Nib[] Nibs => ["[ ]", Prop(Item)];
    public override Joiner Joiner => Joiner.None;

    public T Item { get; set; }
}
