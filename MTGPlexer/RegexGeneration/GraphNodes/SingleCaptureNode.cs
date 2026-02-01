namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class SingleCaptureNode : CaptureNode
{
    protected SingleCaptureNode(Node parentNode, INavigable navigable)
        : base(parentNode, navigable)
    {
    }

    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result)
    {
        var singleCapture = captureDictionary[FullyQualifiedName].SingleOrDefault();

        if (singleCapture == null)
        {
            result = CaptureValueResult.NameNotFound;
            return null;
        }

        var value = GetValueSingleCapture(singleCapture);

        if (value == null)
        {
            result = CaptureValueResult.FoundButNull;
            return null;
        }

        result = CaptureValueResult.FoundWithValue;
        return value;
    }
}
