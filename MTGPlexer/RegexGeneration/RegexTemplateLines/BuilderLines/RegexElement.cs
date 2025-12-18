namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class RegexElement
{
    public Enclosure[] Enclosures { get; }
    public string Regex { get; init; }
    public Palette Palette { init; get; }
    public string Comment { get; init; }

    public string UniquePath { get; }
    public string NamedPath { get; }
    public string NamedPathRelativeToRoot { get; }

    public RegexElement(Enclosure[] enclosures, string regex, Palette palette = null, string comment = null)
    {
        Enclosures = enclosures;
        Regex = regex;
        Palette = palette;
        Comment = comment;
        UniquePath = string.Join('.', enclosures.Select(x => x.Ordinal));
        var namedPathParts = enclosures.OfType<RootEnclosure>().Select(x => x.RootTypeName).Concat(enclosures.OfType<NamedEnclosure>().Select(x => x.Name));
        NamedPath = string.Join('.', namedPathParts);
        NamedPathRelativeToRoot = string.Join(".", namedPathParts.Skip(1));
    }


    public Enclosure[] PropEnclosures => Enclosures.OfType<NamedEnclosure>().ToArray();
    public IEnumerable<Enclosure> VisibleEnclosures => Enclosures.Where(e => e is not RootEnclosure);


    public override string ToString() =>
        Regex
        + (Comment == null ? "" : $" # {Comment}");
}