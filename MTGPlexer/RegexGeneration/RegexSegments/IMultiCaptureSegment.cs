namespace MTGPlexer.RegexGeneration.RegexSegments;

public interface IMultiCaptureSegment
{
    public object GetPropertyValueFromMultiCapture(TokenUnitMatch parentTokenUnitMatch, Capture[] scopedCaptures);
}
