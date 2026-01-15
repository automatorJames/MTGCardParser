namespace MTGPlexer.TokenUnitComponents;

public class OptionalOf<T> : OptionalOf
{
    public PolyItemCapture<T> Item { get; set; }

    public OptionalOf(PolyItemCapture<T> item)
    {
        Item = item;
        ItemObject = item;
    }

    public override string ToString() => base.ToString();

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[Color("#696969")]
public class OptionalOf
{
    public PolyItemCapture ItemObject { get; set; }

    public override string ToString() => string.Join(" ", ItemObject.ToString());

    public static CaptureTypeVariant GetCaptureTypeVariant(Type type) =>
        type.IsAssignableTo(typeof(TokenUnit)) ? CaptureTypeVariant.TokenUnit
        : type.IsEnum ? CaptureTypeVariant.Enum
        : throw new Exception($"{nameof(OptionalOf)} item type must either be TokenUnit or Enum");
}
