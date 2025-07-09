namespace MTGCardParser.Comparers;

class TextSpanAsStringComparer : IEqualityComparer<TextSpan>
{
    public bool Equals(TextSpan x, TextSpan y)
        => x.ToString() == y.ToString();

    public int GetHashCode(TextSpan obj)
        => obj.ToString().GetHashCode();
}

