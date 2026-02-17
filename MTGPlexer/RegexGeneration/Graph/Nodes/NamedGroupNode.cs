namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class NamedGroupNode : GroupNode
{
    public TypeNavigation Navigation { get; }
    public CaptureInfo CaptureValueHydrationInfo { get; protected set; }
    public string FullyQualifiedName { get; }

    protected override RegexBrickBookend GroupOpenBrick => new(this, $"(?<{FullyQualifiedName}>", FullyQualifiedName);
    protected virtual bool OneOrMoreRegexPatternsRequired => false;

    public NamedGroupNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation.Name)
    {
        Navigation = navigation;
        FullyQualifiedName = NamePath.Replace('.', '_');

        if (OneOrMoreRegexPatternsRequired && (navigation.Patterns == null || navigation.Patterns.Length == 0))
            throw new Exception($"'{Name}' is required to have one or more patterns defined via {nameof(RegexPatternAttribute)}");
    }

    //public object GetValueForNamedPath(CaptureContext captureContext)
    //{
    //    var scopedContext = captureContext[this];
    //
    //    if (!scopedContext.Success)
    //        return null;
    //
    //    return GetValue(scopedContext);
    //}
    //
    //public CaptureContext GetScopedContext(CaptureContext context) => context[this];
    //
    //protected abstract object GetValue(CaptureContext context);
    //
    //public bool SetPropertyValue(CaptureContext captureContext, TokenUnit parent)
    //{
    //    if (Navigation is not PropNavigation propNavigation)
    //        throw new Exception($"Navigation for {FullyQualifiedName} is not a {nameof(PropNavigation)}, so it can't set a value on an instance");
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
