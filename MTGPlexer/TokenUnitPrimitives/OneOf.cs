namespace MTGPlexer.TokenUnitPrimitives;

public class OneOf<T1, T2> : TokenUnit
{
    public object Item1 { get; set; }
    public object Item2 { get; set; }

    public OneOf(object item, int capturePropOrdinal)
    {
        var capturedItemType = GetType().GetGenericArguments()[capturePropOrdinal];
        var propToSet = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)[capturePropOrdinal];
        propToSet.SetValue(this, item);
    }
}

public class OneOf<T1, T2, T3> : TokenUnit
{
    public object Item1 { get; set; }
    public object Item2 { get; set; }
    public object Item3 { get; set; }

    public OneOf(object capture, int capturedItemTypeOrdinal)
    {
        var capturedItemType = GetType().GetGenericArguments()[capturedItemTypeOrdinal];
        var propToSet = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)[capturedItemTypeOrdinal];
        propToSet.SetValue(this, capture);
    }
}