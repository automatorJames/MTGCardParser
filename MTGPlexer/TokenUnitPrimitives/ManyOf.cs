using MTGPlexer.TokenUnitPrimitives.Internal;

namespace MTGPlexer.TokenUnitPrimitives;

public class ManyOf<T> : TokenUnit
{
    public override Snippet[] Snippets => [Prop(FirstItem), Prop(SecondPlus), ",?[ ]", Prop(Conjunction), "[ ]", Prop(LastItem)];

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