namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ScalarNode : RegexNode, INamedScalarValue
{
    public object ScalarValue { get; }
    public string FormattedValue { get; }
    public bool IsFirst { get; }

    public ScalarNode(
        RegexNode parentNode, 
        object scalarValue,
        string name,
        bool isFirst = false, 
        bool isSynonym = false) 
        : base(parentNode, name)
    {
        ScalarValue = scalarValue;
        FormattedValue = GetFormattedValue(isFirst);
        IsFirst = isFirst;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(new RegexBrick(this, FormattedValue, null));
    }

    /// <summary>
    /// Gets an indented value preceded by "| " if not the first. Intended to be called
    /// both during initial regex composition and during analytical phases, where an isFirst
    /// value may be passed that's different from the instance's original IsFirst value so that
    /// sibling members may be optionally omitted from the analytic view of the source regex.
    /// </summary>
    string GetFormattedValue(bool isFirst)
    {
        var prefix = isFirst ? "  " : "| ";
        return $"{prefix}{Name}";
    }

    //public RegexBrick GetBrickUpdatedForAnalysis(bool isFirstFiltered, int occurrenceCount) =>
    //    new RegexBrick(ParentNode, GetFormattedValue(isFirstFiltered), $"{CanonicalValue}: {occurrenceCount}");
}
