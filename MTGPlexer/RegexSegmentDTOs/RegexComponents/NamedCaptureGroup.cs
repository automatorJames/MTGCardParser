namespace MTGPlexer.RegexSegmentDTOs.RegexComponents;

public class NamedCaptureGroup : RegexComponentBase
{
    public string Name { get; set; }
    public List<RegexComponentBase> ChildComponents { get; set; } = [];
    public bool IsOptional { get; set; }

    public NamedCaptureGroup(CaptureGroupPropBase captureGroupPropBase)
    {
        Name = captureGroupPropBase.RegexPropInfo.Name;

    }
}

