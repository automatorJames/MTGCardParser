using Glyphotype.GlyphPrimitives.Internal;

namespace Glyphotype.GlyphPrimitives;

public class ManyOf<T> : Glyph
{
    public override Nib[] Nibs => [Prop(FirstItem), Prop(SecondPlus), ",?[ ]", Prop(Conjunction), "[ ]", Prop(LastItem)];

    public override Joiner Joiner => Joiner.None;

    public T FirstItem { get; set; }
    public List<ManyOfSecondItem<T>> SecondPlus { get; set; } = [];
    public Conjunction? Conjunction { get; set; }
    public T LastItem { get; set; }

    public List<T> Items =>
        [ 
            FirstItem, 
            .. SecondPlus?.Select(x => x.Item),
            LastItem 
        ];
}