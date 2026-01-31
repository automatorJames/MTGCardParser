namespace MTGPlexer.RegexGeneration.GraphNodes;

/// <summary>
/// Represents a bool property on a TokenUnit or a bool x-Of PolyItemCapure. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolNode : TerminalNode
{
    public BoolNode(Node parentNode, INavigable navigable) 
        : base(parentNode, navigable)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);
        builder.AddAlternateValues(ScalarAlternateSet.Alternates);
        builder.CloseGroup(GroupQuantifier.Optional);
    }

    protected override object TryGetValue(Capture capture, out CaptureValueResult result)
    {
        // This override simply returns "true", because TerminalNode already validated
        // that the named group exists, therefore this bool check has already succeeded

        result = CaptureValueResult.FoundWithValue;
        return true;
    }
}