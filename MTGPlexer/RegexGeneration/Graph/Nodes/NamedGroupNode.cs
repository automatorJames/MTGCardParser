namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class NamedGroupNode : RegexNode
{
    static HashSet<char> _terminals = ['.', ';', ','];

    public Navigation Navigation { get; }
    public CaptureInfo CaptureValueHydrationInfo { get; protected set; }
    public string FullyQualifiedName { get; }

    protected virtual GroupQuantifier? Quantifier => GetDefaultQuantifier();
    protected RegexBrickBookend GroupOpenBrick => new(this, $"(?<{FullyQualifiedName}>", FullyQualifiedName);
    protected virtual bool OneOrMoreRegexPatternsRequired => false;


    bool _childrenInitialized;
    List<RegexNode> _children;
    public List<RegexNode> Children
    {
        get
        {
            EnsureChildren();
            return _children;
        }
        set
        {
            _children = value ?? throw new ArgumentNullException(nameof(value));
            _childrenInitialized = true;
        }
    }

    protected string QuantifierComment =>
        Quantifier?.ToString().ToFriendlyCase(TitleDisplayOption.Lower);

    protected RegexBrickBookend GroupCloseBrick => new(this, $"){Quantifier?.GetDescription()}", QuantifierComment);
    protected virtual Joiner Joiner => Joiner.None;

    public NamedGroupNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation.Name)
    {
        Navigation = navigation;
        FullyQualifiedName = NamePath.Replace('.', '_');

        if (OneOrMoreRegexPatternsRequired && (navigation.Patterns == null || navigation.Patterns.Length == 0))
            throw new Exception($"'{Name}' is required to have one or more patterns defined via {nameof(RegexPatternAttribute)}");
    }

    GroupQuantifier? GetDefaultQuantifier()
    {
        if (Navigation.IsList)
            return Navigation.Proptions.HasFlag(Proptions.OneOrMore) ? GroupQuantifier.OneOrMore : GroupQuantifier.AnyNumber;
        else
            return null;
    }

    private void EnsureChildren()
    {
        if (_childrenInitialized)
            return;

        _children = new List<RegexNode>();
        AddReflectedChildren(_children);
        _childrenInitialized = true;
    }

    protected virtual void AddReflectedChildren(List<RegexNode> children)
    {
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        // open group
        collector.Append(GroupOpenBrick);

        // append all children and joiners
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].AppendRegexBricks(collector);

            bool shouldAddJoiner =
                i < Children.Count - 1
                && Joiner != Joiner.None
                && !_terminals.Contains(collector.LastChar);

            if (shouldAddJoiner)
                collector.Append(new RegexBrickJoiner(this, Joiner));
        }

        // close group
        collector.Append(GroupCloseBrick); // close group
    }

    public void SetPropertyValue(TokenUnit instance, CaptureContext context)
    {
        var scopedContext = context[this];
        var value = GetValue(scopedContext);
        Navigation.Prop.SetValue(instance, value);
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
    protected abstract object GetValue(CaptureContext context);
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
