namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record RootEnclosure : Enclosure
{
    public string RootTypeName { get; }

    public RootEnclosure(Type rootType) : base(-1, -1, EnclosureType.Root, GroupBorderTreatment.None)
    {
        RootTypeName = rootType.Name;
    }
}
