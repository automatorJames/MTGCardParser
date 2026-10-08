namespace Glyphotype.GlyphPrimitives;

public class CompoundOf<T> : CompoundOfBase, IWrapsItems<T>
{
    public T FirstItem { get; set; }

    [AnyNumber] 
    public List<CompoundOfSecondItem<T>> SecondPlus { get; set; } = [];

    public List<T> Items => [FirstItem, .. SecondPlus.Select(x => x.Item)];

    IEnumerable<object> IWrapsItems.WrappedItems => Items.Cast<object>();
}