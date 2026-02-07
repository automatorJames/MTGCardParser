namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class NamedGroupNode : GroupNode
{
    public string FullyQualifiedName { get; }

    public NamedGroupNode(RegexNode parentNode, INavigable navigable) 
        : base(parentNode, navigable)
    {
        FullyQualifiedName = string.Join("_", Lineage.Where(x => !x.IsCollapsible));
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(new(this, $"(?<{FullyQualifiedName}>", FullyQualifiedName));
        builder.OpenNamedGroup(this);
        builder.AddAlternateValues(ScalarAlternateSet.Alternates);
        collector.Append(new(this, $"){Quantifier?.GetDescription()}", QuantifierComment));
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
