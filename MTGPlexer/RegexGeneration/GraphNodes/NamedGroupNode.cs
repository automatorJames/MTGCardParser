namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class NamedGroupNode : GroupNode
{
    public NamedGroupNode(RegexNode parentNode, INavigable navigable) 
        : base(parentNode, navigable)
    {
    }

    public override void ComposeRegexLines(RegexBuilder collector)
    {
        builder.OpenNamedGroup(this);
        builder.AddAlternateValues(ScalarAlternateSet.Alternates);
        builder.CloseGroup(GroupQuantifier.Optional);
    }

    public bool SetPropertyValue(CaptureContext captureContext, TokenUnit parent)
    {
        if (ConcreteProperty == null)
            throw new Exception($"{FullyQualifiedName} does not represent a concrete CLR property, so its value cannot be set");

        var value = GetValueAndSetHydrationInfo(captureContext);

        if (value == null && AbortIfSetPropertyToNull)
            return false;

        ConcreteProperty.SetValue(parent, value);

        return true;
    }
}
