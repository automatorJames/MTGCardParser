namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class GroupNode : RegexNode
{
    public List<RegexNode> Children { get; protected set; } = [];
    protected virtual RegexBrickBookend GroupOpenBrick => AnonymousGroupOpenBrick;
    protected RegexBrickBookend AnonymousGroupOpenBrick => new (this, "(", null);
    protected RegexBrickBookend GroupCloseBrick => new(this, $"){Quantifier?.GetDescription()}", QuantifierComment);
    protected virtual Joiner Joiner => Joiner.None;
    protected virtual GroupQuantifier? Quantifier => null;

    protected string QuantifierComment => 
        Quantifier?.ToString().ToFriendlyCase(TitleDisplayOption.Lower);

    protected virtual bool AbortIfSetPropertyToNull => false;

    protected GroupNode(RegexNode parentNode, string name)
        : base(parentNode, name)
    {
        //IsOptional = navigable.Proptions.HasFlag(Proptions.Optional)
        //    || UnderlyingType.IsEnum && Nullable.GetUnderlyingType(Navigable.Type) != null;
        //
        //if (navigable is PropertySnippet propertySnippet)
        //{
        //    ConcreteProperty = propertySnippet.Prop;
        //
        //    IsOptional |= ConcreteProperty.IsDefined(typeof(OptionalComponentAttribute));
        //
        //    // Optional sub-groups aren't allowed in TokenUnitOneOf groups, because they would allow zero-width matches
        //    IsOptional &= !ConcreteProperty.DeclaringType.IsAssignableTo(typeof(TokenUnitOneOf));
        //
        //    OverrideRegexPatterns = ConcreteProperty.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;
        //}
    }

    public void SetChildren(params RegexNode[] children) =>
        Children.AddRange(children);

    public override void AppendRegexBricks(RegexCollector collector)
    {
        // open group
        collector.Append(GetGroupOpenBrick());

        // append all children and joiners
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].AppendRegexBricks(collector);

            if (i < Children.Count - 1 && Joiner != Joiner.None)
                if (Joiner == Joiner.Pipe)
                    collector.Append(new RegexBrickAlternatingPipe(this));
                else
                {
                    var regex = Joiner.GetDescription();
                    var comment = $"joiner {Joiner.ToString().ToFriendlyCase(TitleDisplayOption.Lower)}";
                    collector.Append(new RegexBrick(this, regex, comment));
                }
        }

        // close group
        collector.Append(new(this, $"){Quantifier?.GetDescription()}", QuantifierComment)); // close group
    }

    protected virtual RegexBrick GetGroupOpenBrick() =>
        new RegexBrick(this, "(", null);

    protected RegexBrick GetGroupCloseBrick(GroupQuantifier? quantifier = null)
    {
        quantifier ??= Quantifier;
        var comment = GetQuantifierComment(quantifier);
        return new RegexBrick(this, $"){quantifier?.GetDescription()}", comment);
    }

    protected string GetQuantifierComment(GroupQuantifier? quantifier = null)
    {
        quantifier ??= Quantifier;
        return quantifier?.GetDescription();
    }
}
