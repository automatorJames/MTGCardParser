using System.Diagnostics.CodeAnalysis;

namespace MTGPlexer.TokenAnalysis;

public class UnmatchedSpanComparer : IEqualityComparer<SpanContext>
{
    public bool Equals(SpanContext x, SpanContext y) => x.SpanText == y.SpanText;
    public int GetHashCode([DisallowNull] SpanContext obj) => obj.SpanText.GetHashCode();
}