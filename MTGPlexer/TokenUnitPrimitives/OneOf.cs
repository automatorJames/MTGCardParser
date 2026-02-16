namespace MTGPlexer.TokenUnitPrimitives;

public class OneOf<T1, T2> : OneOf
{
    public object Item1 { get; set; }
    public object Item2 { get; set; }

    public OneOf(object item, int capturePropOrdinal)
    {
        var capturedItemType = GetType().GetGenericArguments()[capturePropOrdinal];
        var propToSet = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)[capturePropOrdinal];
        propToSet.SetValue(this, item);
        ItemType = propToSet.PropertyType;
        Item = item;
    }
}

public class OneOf<T1, T2, T3> : OneOf
{
    public object Item1 { get; set; }
    public object Item2 { get; set; }
    public object Item3 { get; set; }

    public OneOf(object capture, int capturedItemTypeOrdinal)
    {
        var capturedItemType = GetType().GetGenericArguments()[capturedItemTypeOrdinal];
        var propToSet = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)[capturedItemTypeOrdinal];
        propToSet.SetValue(this, capture);
        ItemType = propToSet.PropertyType;
        Item = capture;
    }
}


[Color("#696969")]
public class OneOf : XOf
{
    public object Item { get; set; }
    public Type ItemType { get; set; }
}