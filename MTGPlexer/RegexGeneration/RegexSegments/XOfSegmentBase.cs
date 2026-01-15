namespace MTGPlexer.RegexGeneration.RegexSegments;

public abstract record XOfSegmentBase : CaptureGroupSegmentBase
{
    public Type[] GenericTypes { get; set; }
    protected Type GenericType { get; set; }

    public XOfSegmentBase(TemplatePropInfo captureProp) : base(captureProp)
    {
        GenericTypes = captureProp.GenericTypes;
        SetGenericType(captureProp);
    }

    /// <summary>
    /// Most XOfSegmentBase inheritors expect exactly one generic type. Those that expect a different number
    /// may override this method. Using .Single() allows us to early-surface the unexpected condition where
    /// the underlying prop has more than one generic type, which most XOfSegments aren't equipped to handle.
    /// </summary>
    protected virtual void SetGenericType(TemplatePropInfo captureProp)
    {
        GenericType = captureProp.GenericTypes.Single();
    }
}
