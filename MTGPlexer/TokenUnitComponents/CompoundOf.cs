namespace MTGPlexer.TokenUnitComponents;

public class CompoundOf<T> : CompoundOf
{
    public CompoundItemCapture<T>[] Items { get; set; }

    public CompoundOf(IEnumerable<CompoundItemCapture<T>> items)
    {
        Items = items.ToArray();
        ItemObjects = Items.Cast<CompoundItemCapture>().ToList();
        ItemType = typeof(T);
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
public class CompoundOf : IEquatable<CompoundOf>
{
    public Guid DistinctId { get; } = Guid.NewGuid();
    public List<CompoundItemCapture> ItemObjects { get; set; }
    public CaptureTypeVariant CaptureTypeVariant { get; set; }
    public Type ItemType { get; set; }

    public override string ToString() => string.Join(" ", ItemObjects.Select(x => x.ToString()));

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType() && !obj.GetType().IsSubclassOf(typeof(CompoundOf))) return false;
        return Equals((CompoundOf)obj);
    }

    public bool Equals(CompoundOf other)
    {
        if (other is null) return false;

        return ItemObjects
            .Select(i => i.ItemObject.ToString())
            .SequenceEqual(other.ItemObjects.Select(i => i.ItemObject.ToString()));
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (397) ^ (ItemObjects != null ? ItemObjects.Aggregate(0, (acc, item) => acc ^ (item.ItemObject?.ToString().GetHashCode() ?? 0)) : 0);
            return hashCode;
        }
    }

    public static CaptureTypeVariant GetCaptureTypeVariant(Type type) =>
        type.IsAssignableTo(typeof(TokenUnit)) ? CaptureTypeVariant.TokenUnit
        : type.IsEnum ? CaptureTypeVariant.Enum
        : throw new Exception($"{nameof(CompoundOf)} item type must either be TokenUnit or Enum");
}
