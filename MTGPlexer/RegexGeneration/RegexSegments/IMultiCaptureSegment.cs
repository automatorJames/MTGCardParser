namespace MTGPlexer.RegexGeneration.RegexSegments;

public interface IMultiCaptureSegment
{
    public object SetPropertyFromCaptures(TokenUnit parentTokenUnit, Capture[] scopedCaptures);
}
