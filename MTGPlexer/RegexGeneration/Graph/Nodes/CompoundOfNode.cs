namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class CompoundOfNode : WrapperNode
{
    Joiner _joiner;
    public CompoundOfNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        _joiner = navigation.UnderlyingType.GetCustomAttribute<CompoundJoinerAttribute>()?.Joiner
            ?? Joiner.Space;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        collector.AppendJoined(Children, GetJoinerBrick(_joiner));
        collector.Append(GroupCloseBrick);

        builder.OpenNamedGroup(this);
        builder.OpenAnonymousGroup();
        GetTemplateNodeForType().ComposeRegexLines(builder);
        builder.AddTextLine(" ?");
        builder.CloseGroup(GroupQuantifier.OneOrMore);
        builder.AddNegativeSpaceLookbehindBoundary();
        builder.CloseGroup();
    }

    protected override object GetWrapperValue(CaptureContext captureContext)
    {
        var scopedCaptureContext = captureContext[FullyQualifiedName];

        // We expect two or more captures
        if (scopedCaptureContext.Captures.Length <= 1)
            return null;

        for (int i = 0; i < scopedCaptureContext.Captures.Length; i++)
        {
            var singleScopedContext = scopedCaptureContext.ScopeToCaptureIndex(i);
            AddNewWrappedNode(singleScopedContext, ordinal: i);
        }

        return CreateWrapperValue();
    }
}