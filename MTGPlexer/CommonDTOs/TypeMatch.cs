namespace MTGPlexer.CommonDTOs;

public record TypeMatch
(
    Match Match = null, 
    CaptureGroupPropPath CapturePath = null, 
    string DistinguishingAppendix = null, 
    int CaptureIndex = 0
);
