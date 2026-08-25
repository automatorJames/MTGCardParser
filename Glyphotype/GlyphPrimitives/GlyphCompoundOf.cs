using Glyphotype.GlyphPrimitives.Internal;

namespace Glyphotype.GlyphPrimitives;

public class CompoundOf<T> : CompoundOfBase
{
    public T FirstItem { get; set; }

    [AnyNumber] 
    public List<CompoundOfSecondItem<T>> SecondPlus { get; set; } = [];

    public List<T> Items => [FirstItem, .. SecondPlus.Select(x => x.Item)];
}