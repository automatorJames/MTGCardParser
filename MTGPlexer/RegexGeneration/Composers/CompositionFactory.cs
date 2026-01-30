namespace MTGPlexer.RegexGeneration.Composers;

public static class CompositionFactory
{
    public static RegexBuilder Compose(IEnumerable<Node> nodes, Type rootType)
    {
        var nodeList = nodes.ToList();
        RegexBuilder builder = new(rootType);

        if (rootType.IsAssignableTo(typeof(TokenUnitOneOf)))
            AlternatingComposer.Instance.Compose(builder, nodeList);
        else if (rootType.IsAssignableTo(typeof(TokenUnit)))
            ConcatenatingComposer.Instance.Compose(builder, nodeList);

        return builder;
    }


    public static string GetComposedString(IEnumerable<Node> nodes, Type rootType)
    {
        var builder = Compose(nodes, rootType);
        return builder.GetMinified();
    }

}
