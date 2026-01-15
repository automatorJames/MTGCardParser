namespace MTGPlexer.TokenUnitComponents;

public class CompoundOf<T> : CompoundOf
{
    public PolyItemCapture<T>[] Items { get; set; }

    public CompoundOf(IEnumerable<PolyItemCapture<T>> items)
    {
        Items = items.ToArray();
        ItemObjects = Items.Cast<PolyItemCapture>().ToList();
        CaptureTypeVariant = GetCaptureTypeVariant(typeof(T));
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
public class CompoundOf
{
    public List<PolyItemCapture> ItemObjects { get; set; }
    public CaptureTypeVariant CaptureTypeVariant { get; set; }

    public override string ToString() => string.Join(" ", ItemObjects.Select(x => x.ToString()));

    public static CaptureTypeVariant GetCaptureTypeVariant(Type type) =>
        type.IsAssignableTo(typeof(TokenUnit)) ? CaptureTypeVariant.TokenUnit
        : type.IsEnum ? CaptureTypeVariant.Enum
        : throw new Exception($"{nameof(CompoundOf)} item type must either be TokenUnit or Enum");
}
