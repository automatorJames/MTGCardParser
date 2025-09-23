namespace MTGPlexer.RegexGeneration.RegexTemplateLines.PathElements;

public record RootEnclosure : Enclosure
{
    public RootEnclosure() : base(-1, null, EnclosureType.Root, GroupBorderTreatment.None) { }
}
