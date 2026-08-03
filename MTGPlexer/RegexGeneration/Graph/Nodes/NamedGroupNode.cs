using System.Collections;

namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class NamedGroupNode : GroupNode
{
    protected virtual string DefaultPattern => null;

    static HashSet<char> _terminals = ['.', ';', ','];
    public bool IsTransparentRoot => Navigation.IsRoot && Navigation.Quantifier == null;
    public abstract CaptureNodeType NodeType { get; }

    protected override Quantifier? Quantifier =>
        Navigation.IsList ? MTGPlexer.Quantifier.AnyNumber
        : base.Quantifier;

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
        Navigation.Quantifier?.ToString().ToFriendlyCase(TitleDisplayOption.Lower);

    protected virtual Joiner Joiner => Joiner.None;

    public NamedGroupNode(RegexNode parentNode, Navigation navigation) 
        : base(parentNode, navigation)
    {
        if (OneOrMoreRegexPatternsRequired && (navigation.Patterns == null || navigation.Patterns.Length == 0))
            throw new Exception($"'{Name}' is required to have one or more patterns defined via {nameof(RegexPatternAttribute)}");
    }

    protected RegexBrickGroupOpen GetGroupOpenBrick() =>
        new (
            parentNode: this, 
            groupName: FullyQualifiedName, 
            comment: FullyQualifiedName);

    protected RegexBrickGroupClose GetGroupCloseBrick() =>
        new (
            parentNode: this, 
            quantifier: Navigation.IsOptional ? MTGPlexer.Quantifier.Optional : Quantifier,
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
        string[] patterns = Navigation.Patterns;

        if (patterns == null)
        {
            if (DefaultPattern == null)
                throw new Exception($"Either {nameof(Navigation.Patterns)} or {nameof(DefaultPattern)} must be non-null");
            else
                patterns = [DefaultPattern];
        }

        children.AddRange(
        patterns.Select((x, idx) => new TerminalRegexNode(
            parentNode: this,
            name: $"{GetType().Name}-{idx}",
            regexString: x
        )));
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
            var joiner = Navigation.TokenTypeConfiguration?.Joiner ?? Joiner;

            bool shouldAddJoiner =
                i < Children.Count - 1
                && joiner != Joiner.None
                && collector.LastChar != ' '
                && !_terminals.Contains(collector.LastChar);

            if (shouldAddJoiner)
                collector.Append(new RegexBrickJoiner(this, joiner));
        }

        // close group
        if (!IsTransparentRoot)
            collector.Append(GetGroupCloseBrick());
    }

    public virtual bool SetPropertyValue(TokenUnit instance, CaptureContext context)
    {
        object value;
        var captureInfo = context[this];

        // todo: for certain navigations, scopedContext.Success should be required for (excepting OneOf, OptionalOf, etc.)
        // Therefore we should enforce this as necessary to avoid hard-to-isolate silent failure modes where hydration succeeds without
        // throwing an exception, but is missing most or all of its property values
        if (!captureInfo.Success)
            return false;

        if (Navigation.IsList)
        {
            var listType = typeof(List<>).MakeGenericType(Navigation.GenericTypes[0]);
            var list = (IList)Activator.CreateInstance(listType);
            
            foreach (var sibling in captureInfo)
            {
                var itemValue = GetValue(sibling);
                sibling.ClrValue = itemValue;
                list.Add(itemValue);
            }

            value = list;
        }
        else
        {
            value = GetValue(captureInfo);
            captureInfo.ClrValue = value;
        }

        if (value == null)
            return false;

        // Assign the value to the prop (either a single object value or a List<object> value)
        Navigation.Prop.SetValue(instance, value);

        return true;
    }

    protected abstract object GetValue(CaptureTrace captureInfo);
}
