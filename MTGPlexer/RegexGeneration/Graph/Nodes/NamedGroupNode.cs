namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class NamedGroupNode : GroupNode
{
    public TypeNavigation Navigation { get; }
    public CaptureValueHydrationInfo CaptureValueHydrationInfo { get; protected set; }
    public string FullyQualifiedName { get; }

    protected virtual bool OneOrMoreRegexPatternsRequired => false;

    public NamedGroupNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation.Name)
    {
        Navigation = navigation;
        FullyQualifiedName = NamePath.Replace('.', '_');
        Children = GetChildNodes();

        if (OneOrMoreRegexPatternsRequired && (navigation.Patterns == null || navigation.Patterns.Length == 0))
            throw new Exception($"'{Name}' is required to have one or more patterns defined via {nameof(RegexPatternAttribute)}");
    }

    protected abstract List<RegexNode> GetChildNodes();

    public override void AppendRegexBricks(RegexCollector collector)
    {
        Children.ForEach(x => x.AppendRegexBricks(collector));
    }

    protected override RegexBrick GetGroupOpenBrick() =>
        new(this, $"(?<{FullyQualifiedName}>", FullyQualifiedName);

    //public bool SetPropertyValue(CaptureContext captureContext, TokenUnit parent)
    //{
    //    if (ConcreteProperty == null)
    //        throw new Exception($"{FullyQualifiedName} does not represent a concrete CLR property, so its value cannot be set");
    //
    //    var value = GetValueAndSetHydrationInfo(captureContext);
    //
    //    if (value == null && AbortIfSetPropertyToNull)
    //        return false;
    //
    //    ConcreteProperty.SetValue(parent, value);
    //
    //    return true;
    //}
}
