using MTGPlexer.TokenUnitPrimitives.Internal;

namespace MTGPlexer.TokenUnitPrimitives;

public class CompoundOf<T> : TokenUnit
{
    protected override Snippet[] Snippets => [];
    public override Joiner Joiner => Joiner.None;

    public T FirstItem { get; set; }

    [OneOrMore] 
    public List<CompoundOfSecondItem<T>> SecondPlus { get; set; } = [];
}