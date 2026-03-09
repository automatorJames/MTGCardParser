using System.Collections;

namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class NamedGroupNode : RegexNode
{
    static HashSet<char> _terminals = ['.', ';', ','];
    public bool IsTransparentRoot => Navigation.IsRoot && Quantifier == null;

    public Navigation Navigation { get; }
    public CaptureInfo CaptureValueHydrationInfo { get; protected set; }
    public string FullyQualifiedName { get; }

    protected virtual GroupQuantifier? Quantifier => GetDefaultQuantifier();
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

    protected RegexBrickGroupOpen GetGroupOpenBrick() =>
        new (
            parentNode: this, 
            groupName: FullyQualifiedName, 
            comment: FullyQualifiedName);

    protected RegexBrickGroupClose GetGroupCloseBrick() =>
        new (
            parentNode: this, 
            quantifier: Quantifier,
            comment: QuantifierComment);

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
        if (!IsTransparentRoot)
            collector.Append(GetGroupOpenBrick());

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
        if (!IsTransparentRoot)
            collector.Append(GetGroupCloseBrick());
    }

    public virtual void SetPropertyValue(TokenUnit instance, CaptureContext context)
    {
        object value = null;
        var scopedContext = context[this];

        if (Navigation.IsList)
        {
            var listType = typeof(List<>).MakeGenericType(Navigation.GenericTypes[0]);
            var list = (IList)Activator.CreateInstance(listType);
            
            for (int i = 0; i < scopedContext.Count; i++)
            {
                var captureScopedContext = scopedContext.ScopeToCaptureIndex(i);
                var itemValue = GetValue(captureScopedContext);
                list.Add(itemValue);
            }

            value = list;
        }
        else
            value = GetValue(scopedContext);

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
