using System.Diagnostics.CodeAnalysis;

namespace MTGCardParser.TokenTesting;

public class PropertyCaptureComparer : IEqualityComparer<PropertyCapture>
{
    public bool Equals(PropertyCapture x, PropertyCapture y) => x.Prop == y.Prop;
    public int GetHashCode([DisallowNull] PropertyCapture obj) => obj.Prop.GetHashCode();
}

