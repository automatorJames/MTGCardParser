namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class GroupNode : BranchNode
{
    protected virtual RegexBrickBookend GroupOpenBrick => AnonymousGroupOpenBrick;
    protected RegexBrickBookend AnonymousGroupOpenBrick => new (this, "(", null);
    protected RegexBrickBookend GroupCloseBrick => new(this, $"){Quantifier?.GetDescription()}", QuantifierComment);
    protected virtual GroupQuantifier? Quantifier => null;

    protected string QuantifierComment => 
        Quantifier?.ToString().ToFriendlyCase(TitleDisplayOption.Lower);

    protected virtual bool AbortIfSetPropertyToNull => false;

    protected GroupNode(RegexNode parentNode, TypeNavigation navigation)
        : base(parentNode, navigation)
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

    protected GroupNode(RegexNode parentNode, TypeNavigation navigation)
    : base(parentNode, navigation)
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

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GetGroupOpenBrick()); // open group
        base.AppendRegexBricks(collector); // all children append their bricks
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
