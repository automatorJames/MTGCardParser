namespace MTGPlexer.RegexGeneration.GraphNodes;

/// <summary>
/// Represents a bool property on a TokenUnit or a bool x-Of PolyItemCapure. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolNode : TerminalNode
{
    public BoolNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this, isOptional: true);
        builder.AddAlternateValues(ScalarAlternateSet.Alternates);
        builder.CloseGroup(GroupQuantifier.Optional);
    }

    public override object GetValue(Capture capture)
    {
        // This override simply returns "true", because CaptureGroupPropBase already validated
        // that the named group exists, therefore this bool check has already succeeded

        return true;
    }
}