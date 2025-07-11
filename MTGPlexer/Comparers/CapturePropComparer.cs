using System.Diagnostics.CodeAnalysis;

namespace MTGPlexer.TokenTesting;

public class CapturePropComparer : IEqualityComparer<RegexPropInfo>
{
    public bool Equals(RegexPropInfo x, RegexPropInfo y) => x.Prop == y.Prop;
    public int GetHashCode([DisallowNull] RegexPropInfo obj) => obj.Prop.GetHashCode();
}