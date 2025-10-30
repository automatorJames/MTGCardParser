namespace MTGPlexer.TokenUnitComponents;

public class ManyOf<T> : ManyOf
{
    public ManyItemCapture<T>[] Items { get; set; }

    public ManyOf(IEnumerable<ManyItemCapture<T>> items, Conjunction? conjunction, Capture conjunctionCapture)
    {
        Items = items.ToArray();
        ItemObjects = Items.Cast<ManyItemCapture>().ToList();
        Conjunction = conjunction;
        ConjunctionCapture = conjunctionCapture;
        ItemType = typeof(T);
        ManyItemVariant = GetManyItemVariant(typeof(T));
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

public enum Conjunction
{
    And,
    Or
}

[Color("#696969")]
public class ManyOf : IEquatable<ManyOf>
{
    public Guid DistinctId { get; } = Guid.NewGuid();
    public List<ManyItemCapture> ItemObjects { get; set; }
    public ManyItemVariant ManyItemVariant { get; set; }
    public Type ItemType { get; set; }
    public Conjunction? Conjunction { get; set; }
    public Capture ConjunctionCapture { get; set; }

    public override string ToString()
    {
        var separator = Conjunction switch
        {
            TokenUnitComponents.Conjunction.And => " & ",
            TokenUnitComponents.Conjunction.Or => " | ",
            _ => " & ",
        };

        return string.Join(separator, ItemObjects.Select(x => x.ItemObject.ToString()));
    }

    /// <summary>
    /// Used for GUI display purposes. Returns a space-separated path to each terminal value
    /// (first, secondPlus, last, and conjunction).
    /// </summary>
    public string GetJoinedPathForAllTerminals(CaptureGroupPropPath parentPath)
    {
        string stringPath = "";

        stringPath += string.Join(' ', ItemObjects.Select(x =>
            parentPath.PropPathRelativeToRoot
            + "."
            + parentPath.LeafName + x.Oridinal.Description()
            + "."
            + x.ItemObject.ToString()));

        if (Conjunction.HasValue)
            stringPath += 
                " " 
                + parentPath.PropPathRelativeToRoot 
                + "." 
                + nameof(Conjunction) 
                + "." 
                + Conjunction.ToString();

        return stringPath;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType() && !obj.GetType().IsSubclassOf(typeof(ManyOf))) return false;
        return Equals((ManyOf)obj);
    }

    public bool Equals(ManyOf other)
    {
        if (other is null) return false;

        if (Conjunction != other.Conjunction) return false;

        return ItemObjects
            .Select(i => i.ItemObject.ToString())
            .SequenceEqual(other.ItemObjects.Select(i => i.ItemObject.ToString()));
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (Conjunction != null ? Conjunction.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ItemObjects != null ? ItemObjects.Aggregate(0, (acc, item) => acc ^ (item.ItemObject?.ToString().GetHashCode() ?? 0)) : 0);
            return hashCode;
        }
    }

    public static ManyItemVariant GetManyItemVariant(Type type) =>
        type.IsAssignableTo(typeof(TokenUnit)) ? ManyItemVariant.TokenUnit
        : type.IsEnum ? ManyItemVariant.Enum
        : throw new Exception($"{nameof(ManyOf)} item type must either be TokenUnit or Enum");
}

public enum ManyItemVariant
{
    TokenUnit,
    Enum
}