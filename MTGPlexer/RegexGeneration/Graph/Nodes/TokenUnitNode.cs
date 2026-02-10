namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TokenUnitNode : NamedGroupNode
{
    public TokenUnitNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        collector.AppendJoined(Children, GetJoinerBrick(Joiner.Space));
        collector.Append(GroupCloseBrick);
    }

    //public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    //{
    //    var scopedCaptureContext = captureContext[FullyQualifiedName];
    //
    //    if (!scopedCaptureContext.Success)
    //        return null;
    //
    //    var instance = (TokenUnit)Activator.CreateInstance(UnderlyingType);
    //
    //    foreach (var captureNode in NamedGroupNodes)
    //    {
    //        // will return false only if an underlying property has AbortIfSetPropertyToNull == true
    //        // and the property value is null
    //        var setSuccessfully = captureNode.SetPropertyValue(scopedCaptureContext, instance);
    //
    //        if (!setSuccessfully)
    //            return null;
    //    }
    //
    //    CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, instance);
    //
    //    return instance;
    //}
}
