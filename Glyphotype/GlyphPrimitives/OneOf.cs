namespace Glyphotype.GlyphPrimitives;

public class OneOf<T1, T2> : OneOfBase
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }

    public OneOf()
    {
    }

    public OneOf(object item, int capturePropOrdinal)
    {
        var capturedItemType = GetType().GetGenericArguments()[capturePropOrdinal];
        var propToSet = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)[capturePropOrdinal];
        propToSet.SetValue(this, item);
    }
}

public class OneOf<T1, T2, T3> : OneOfBase
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
    public T3 Item3 { get; set; }

    public OneOf()
    {
    }

    public OneOf(object capture, int capturedItemTypeOrdinal)
    {
        var capturedItemType = GetType().GetGenericArguments()[capturedItemTypeOrdinal];
        var propToSet = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)[capturedItemTypeOrdinal];
        propToSet.SetValue(this, capture);
    }
}