namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public interface IGroupOpen
{
    public Enclosure[] Enclosures { get; }
    public Enclosure[] ParentEnclosures => Enclosures.Length <= 1 ? [] : Enclosures.Take(Enclosures.Length - 1).ToArray();
}
