namespace MTGPlexer.RegexGeneration.Graph;

public interface INamedScalarValue
{
    public Regex Regex { get; }
    public string Name { get; }
    public object ScalarValue { get; }
    //public string FullyQualifiedName { get; }
}
