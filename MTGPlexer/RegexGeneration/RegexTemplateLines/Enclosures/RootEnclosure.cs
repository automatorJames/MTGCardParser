namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record RootEnclosure : Enclosure
{
    public Type TopLevelType { get; }
    public string RootTypeName { get; }

    public RootEnclosure(Type rootType) : base(-1, -1, null, EnclosureType.Root, GroupBorderTreatment.None)
    {
        TopLevelType = rootType;
        RootTypeName = rootType.Name;
    }
}
