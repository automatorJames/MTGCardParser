using System.Diagnostics.CodeAnalysis;

namespace MTGCardParser.TokenTesting;

public class CapturePropComparer : IEqualityComparer<CaptureProp>
{
    public bool Equals(CaptureProp x, CaptureProp y) => x.Prop == y.Prop;
    public int GetHashCode([DisallowNull] CaptureProp obj) => obj.Prop.GetHashCode();
}

