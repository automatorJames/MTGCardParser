namespace MTGPlexer.RegexGeneration.Graph;

public interface INamedScalarValue
{
    public string Name { get; }
    public object ScalarValue { get; }
}
