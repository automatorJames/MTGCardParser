namespace MTGPlexer.TokenUnitGraphComponents;

public class DynamicOf<T> : DynamicOf
{
    public DynamicOf(object item)
    {
        Item = item;
    }
}

[Color("#696969")]
public class DynamicOf : XOf
{
    public object Item { get; protected set; }
}