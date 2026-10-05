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

    /// <summary>True for the graph's root node when it has no quantifier — its own group wrapper is skipped since it would be redundant with the compiled regex's own boundaries.</summary>
    public bool IsTransparentRoot => Navigation.IsRoot && Navigation.Quantifier == null;

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

    /// <summary>The joiner this group actually renders between its own children: a Glyph type's declared <see cref="GlyphTypeConfiguration.ChildJoiner"/> if this is a Glyph-typed navigation, else this node type's own <see cref="ChildJoiner"/> override.</summary>
    public Joiner EffectiveChildJoiner => Navigation.GlyphTypeConfiguration?.ChildJoiner ?? ChildJoiner ?? Joiner.None;

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
        new(parentNode: this, quantifier: Quantifier);

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

    /// <summary>
    /// Appends this group's open bookend, its content (see <see cref="AppendInnerContentBricks"/>), and its
    /// close bookend. When this group is <see cref="RegexNode.IsNullable"/>, its own leading and/or trailing
    /// joiner (see <see cref="JoinerRules.PlaceLeadingJoiner"/> and <see cref="JoinerRules.OwnsTrailingJoiner"/>)
    /// are appended here, as this group's own first/last inner content - inside its bookends - so either
    /// joiner's presence is coupled to whichever conditional match already governs this group, rather than
    /// rendering unconditionally.
    /// </summary>
    /// <remarks>
    /// When this group's content is a bare alternation (see <see cref="ContentIsBareAlternation"/>) and it carries a
    /// joiner inside its bookends, the content is grouped - as <see cref="GlyphOneOfNode"/> groups its alternatives
    /// between literal text - so the joiner binds to every alternative rather than only the first or last:
    /// <c>(?&lt;x&gt;[ ](a|b))?</c>, not <c>(?&lt;x&gt;[ ]a|b)?</c>.
    /// </remarks>
    protected override void AppendOwnRegexBricks(RegexCollector collector)
    {
        if (!IsTransparentRoot)
            collector.Append(GetGroupOpenBrick());

        var successor = TrailingJoinerSuccessor;
        var groupsContent = ContentIsBareAlternation && (LeadingJoinerPlacement == JoinerPlacement.InsideNodeLeading || successor is not null);

        _innerLeadingJoiner = LeadingJoinerPlacement == JoinerPlacement.InsideNodeLeading
            ? AppendJoinerBefore(this, collector, owner: this)
            : Joiner.None;

        if (groupsContent)
            collector.Append(new RegexBrick(this, "("));

        AppendInnerContentBricks(collector);

        if (groupsContent)
            collector.Append(new RegexBrick(this, ")"));

        _innerTrailingJoiner = successor is not null
            ? AppendJoinerBefore(successor, collector, owner: this)
            : Joiner.None;

        if (!IsTransparentRoot)
            collector.Append(GetGroupCloseBrick());
    }

    Joiner _innerLeadingJoiner;
    Joiner _innerTrailingJoiner;

    /// <summary>Whether this group's content is two or more alternatives with nothing around them, so anything placed next to it inside the group would bind to one alternative alone.</summary>
    protected virtual bool ContentIsBareAlternation =>
        EffectiveChildJoiner == Joiner.Pipe && Children.Count > 1;

    /// <summary>
    /// What <paramref name="captureTrace"/> captured of this group's own content: its text without the joiners this
    /// group carries inside its own bookends when it's nullable (see <see cref="AppendOwnRegexBricks"/>) - e.g.
    /// "has" rather than " has" - which is what a terminal's value is read from.
    /// </summary>
    protected string ContentText(CaptureTrace captureTrace)
    {
        var text = captureTrace.CaptureValue;
        var leading = JoinerText(_innerLeadingJoiner);
        var trailing = JoinerText(_innerTrailingJoiner);

        if (leading.Length > 0 && text.StartsWith(leading, StringComparison.Ordinal))
            text = text[leading.Length..];

        if (trailing.Length > 0 && text.EndsWith(trailing, StringComparison.Ordinal))
            text = text[..^trailing.Length];

        return text;
    }

    /// <summary>The literal text a joiner matches: "[ ]" is a space.</summary>
    static string JoinerText(Joiner joiner) =>
        joiner == Joiner.None ? "" : joiner.GetDescription().Replace("[ ]", " ");

    /// <summary>Appends everything between this group's open and close bookends - by default, each child in turn (each responsible for its own leading joiner; see <see cref="RegexNode.AppendRegexBricks"/>). Override to inject additional content, e.g. a leading separator that isn't a plain sibling joiner (see <see cref="JoinedItemNode"/>).</summary>
    protected virtual void AppendInnerContentBricks(RegexCollector collector)
    {
        foreach (var child in Children)
            child.AppendRegexBricks(collector);
    }

    /// <summary>
    /// Hydrates this node's captured value(s) from <paramref name="scope"/>'s own <see cref="CaptureTrace.CaptureContext"/>
    /// and assigns them to <paramref name="instance"/>'s corresponding property. Returns false if the
    /// capture was unsuccessful or hydrated to null. <paramref name="scope"/> is the specific ancestor
    /// repetition currently being hydrated (see <see cref="CaptureTrace.WithinScope"/>) - for a node whose
    /// own group never sits inside a repeated ancestor, this is simply whatever ancestor trace called in.
    /// </summary>
    public virtual bool SetPropertyValue(Glyph instance, CaptureTrace scope)
    {
        object value;
        var captureTrace = scope.CaptureContext[this].WithinScope(scope);

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
