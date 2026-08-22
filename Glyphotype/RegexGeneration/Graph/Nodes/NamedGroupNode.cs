using System.Collections;

namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// A node that renders as its own named capture group (e.g. <c>(?&lt;name&gt;...)</c>), and knows how to
/// hydrate a CLR value back out of whatever that group captured. Most of the concrete node types in this
/// namespace derive from this class.
/// </summary>
public abstract class NamedGroupNode : GroupNode
{
    /// <summary>The regex pattern to use when no <see cref="RegexPatternAttribute"/> patterns are declared for this node.</summary>
    protected virtual string DefaultPattern => null;

    static HashSet<char> _terminals = ['.', ';', ','];

    /// <summary>True for the graph's root node when it has no quantifier — its own group wrapper is skipped since it would be redundant with the compiled regex's own boundaries.</summary>
    public bool IsTransparentRoot => Navigation.IsRoot && Navigation.Quantifier == null;

    /// <summary>What kind of capture this node represents (enum, bool, token unit, etc.), used by formatting and by <see cref="CaptureTrace"/>.</summary>
    public abstract CaptureNodeKind NodeKind { get; }

    public override Quantifier? Quantifier =>
        Navigation.IsList ? Navigation.ListQuantifier
        : base.Quantifier;

    /// <summary>True when this node's regex must come from one or more <see cref="RegexPatternAttribute"/>-declared patterns rather than falling back to <see cref="DefaultPattern"/>.</summary>
    protected virtual bool OneOrMoreRegexPatternsRequired => false;

    bool _childrenInitialized;
    List<RegexNode> _children;

    /// <summary>This node's child nodes, computed lazily on first access via <see cref="AddReflectedChildren"/>.</summary>
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

    public List<NamedGroupNode> NamedGroupChildren => Children.OfType<NamedGroupNode>().ToList();

    /// <summary>How sibling children are joined together in the regex (e.g. concatenated, alternated with a pipe).</summary>
    protected virtual Joiner? ChildJoiner => null;

    public NamedGroupNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
        if (OneOrMoreRegexPatternsRequired && (navigation.Patterns == null || navigation.Patterns.Length == 0))
            throw new Exception($"'{Name}' is required to have one or more patterns defined via {nameof(RegexPatternAttribute)}");
    }

    /// <summary>Builds this group's opening brick (e.g. <c>(?&lt;name&gt;</c>). Display comment text is assigned later by the Presentation layer.</summary>
    protected RegexBrickGroupOpen GetGroupOpenBrick() =>
        new(parentNode: this, groupName: FullyQualifiedName);

    /// <summary>Builds this group's closing brick (e.g. <c>)</c> or <c>)*</c>). Display comment text is assigned later by the Presentation layer.</summary>
    protected RegexBrickGroupClose GetGroupCloseBrick() =>
        new(parentNode: this, quantifier: Navigation.IsOptional ? Glyphotype.Quantifier.Optional : Quantifier);

    private void EnsureChildren()
    {
        if (_childrenInitialized)
            return;

        _children = new List<RegexNode>();
        AddReflectedChildren(_children);
        _childrenInitialized = true;
    }

    /// <summary>
    /// Populates this node's children. The default implementation adds one <see cref="TerminalRegexNode"/>
    /// per declared/default pattern; subclasses that reflect over a type's properties (e.g. <see cref="GlyphNode"/>, <see cref="EnumNode"/>) override this instead.
    /// </summary>
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

    /// <summary>Appends this group's open bookend, its children (joined per <see cref="ChildJoiner"/>), and its close bookend.</summary>
    public override void AppendRegexBricks(RegexCollector collector)
    {
        // open group
        if (!IsTransparentRoot)
            collector.Append(GetGroupOpenBrick());

        // append all children and joiners
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            child.AppendRegexBricks(collector);
            var joiner = Navigation.GlyphTypeConfiguration?.ChildJoiner ?? ChildJoiner ?? Joiner.None;

            bool shouldAddJoiner =
                i < Children.Count - 1
                && joiner != Joiner.None
                && collector.LastChar != ' '
                && !(Children[i + 1] is TextNode textNode && textNode.FirstChar == '\'');
                //&& !_terminals.Contains(collector.LastChar);

            if (shouldAddJoiner)
                collector.Append(new RegexBrickJoiner(this, joiner));
        }

        // close group
        if (!IsTransparentRoot)
            collector.Append(GetGroupCloseBrick());
    }

    /// <summary>Hydrates this node's captured value(s) from <paramref name="context"/> and assigns them to <paramref name="instance"/>'s corresponding property. Returns false if the capture was unsuccessful or hydrated to null.</summary>
    public virtual bool SetPropertyValue(Glyph instance, CaptureContext context)
    {
        object value;
        var captureTrace = context[this];

        // todo: for certain navigations, scopedContext.Success should be required for (excepting OneOf, OptionalOf, etc.)
        // Therefore we should enforce this as necessary to avoid hard-to-isolate silent failure modes where hydration succeeds without
        // throwing an exception, but is missing most or all of its property values
        if (!captureTrace.Success)
            return false;

        if (Navigation.IsList)
        {
            var listType = typeof(List<>).MakeGenericType(Navigation.GenericTypes[0]);
            var list = (IList)Activator.CreateInstance(listType);
            
            foreach (var sibling in captureTrace)
            {
                var itemValue = GetValue(sibling);
                sibling.ClrValue = itemValue;
                list.Add(itemValue);
            }

            value = list;
        }
        else
        {
            value = GetValue(captureTrace);
            captureTrace.ClrValue = value;
        }

        if (value == null)
            return false;

        // Assign the value to the prop (either a single object value or a List<object> value)
        Navigation.Prop.SetValue(instance, value);

        return true;
    }

    /// <summary>Converts one successful capture into the CLR value this node's property should hold.</summary>
    protected abstract object GetValue(CaptureTrace captureTrace);
}
