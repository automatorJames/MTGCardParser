namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TokenUnitNode : NamedGroupNode
{
    public List<NamedGroupNode> ContiguousCoreNodes { get; set; }

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

    /// <summary>
    /// Validates the capture structure based on two rules:
    /// 1. There must be at least one CaptureNode present.
    /// 2. All CaptureNodes must form a single, contiguous block (no gaps allowed).
    /// </summary>
    /// <returns>True if exactly one contiguous group of CaptureNodes exists.</returns>
    public bool ValidateCapturePropertiesAreContiguous()
    {
        int groups = 0;
        bool inGroup = false;

        foreach (var node in Children)
        {
            if (node is NamedGroupNode)
            {
                // If we hit a capture and weren't already in a group, 
                // we've discovered a new "island"
                if (!inGroup)
                {
                    groups++;
                    inGroup = true;
                }
            }
            else
            {
                // Any non-capture node (TextNode, etc.) terminates the current group
                inGroup = false;
            }
        }

        // Returns true only if we found exactly one contiguous cluster
        return groups == 1;
    }
}
