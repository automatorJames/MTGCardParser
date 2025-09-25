namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record RootEnclosure : Enclosure
{
    public string RootTypeName { get; }

    public RootEnclosure(string rootTypeName) : base(-1, null, EnclosureType.Root, GroupBorderTreatment.None)
    {
        RootTypeName = rootTypeName;
    }
}
