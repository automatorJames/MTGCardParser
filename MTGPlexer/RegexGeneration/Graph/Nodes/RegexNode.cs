namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class RegexNode
{
    public string Name { get; }
    public string NamePath { get; }
    public RegexNode ParentNode { get; }
    public RegexNode[] Lineage { get; }

    protected RegexNode(RegexNode parentNode, string name)
    {
        Name = name;
        ParentNode = parentNode;
        Lineage = GetLineage();
        NamePath = string.Join('.', Lineage.Select(x => x.Name));
    }

    // todo: This feels like a hack that prevents duplicate parts in name paths
    // used only when WrappedNodes are in play rather than a univerasal necessity
    public virtual bool IsCollapsible => false;

    public static NamedGroupNode GetNamedGroupChild(
        RegexNode parentNode,
        PropNavigation wrapperPropNavigation,
        Type typeToWrap,
        string groupNameAppendix)
    {
        var wrappedName = parentNode.Name + "_" + groupNameAppendix;
        TypeNavigation navigation = new(typeToWrap, wrappedName, wrapperPropNavigation.Patterns);

        NamedGroupNode wrappedNamedGroupChild = typeToWrap.GetUnderlyingType() switch
        {
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when typeof(TokenUnitCompound).IsAssignableFrom(t) => new TokenUnitCompoundNode(parentNode, navigation),
            { } t when typeof(TokenUnitOneOf).IsAssignableFrom(t) => new TokenUnitOneOfNode(parentNode, navigation),
            { } t when typeof(TokenUnit).IsAssignableFrom(t) => new TokenUnitNode(parentNode, navigation),
            _ => throw new Exception($"'{typeToWrap}' is not an enum or a {nameof(TokenUnit)} type")
        };

        return wrappedNamedGroupChild;
    }

    public abstract void AppendRegexBricks(RegexCollector collector);

    RegexNode[] GetLineage()
    {
        List<RegexNode> lineage = [];
        RegexNode current = this;

        while (current != null)
        {
            lineage.Add(current);
            current = current.ParentNode;
        }

        lineage.Reverse();
        return lineage.ToArray();
    }

    public override string ToString() => NamePath;
}