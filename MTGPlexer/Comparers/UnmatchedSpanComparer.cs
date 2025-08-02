using System.Diagnostics.CodeAnalysis;

namespace MTGPlexer.TokenAnalysis;

public class UnmatchedSpanComparer : IEqualityComparer<UnmatchedSpanContext>
{
    public bool Equals(UnmatchedSpanContext x, UnmatchedSpanContext y) => x.SpanText == y.SpanText;
    public int GetHashCode([DisallowNull] UnmatchedSpanContext obj) => obj.SpanText.GetHashCode();
}