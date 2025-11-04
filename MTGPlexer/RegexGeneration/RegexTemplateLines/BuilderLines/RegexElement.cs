namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class RegexElement
{
    public Enclosure[] Enclosures { get; }
    public string Regex { get; init; }
    public Palette Palette { init; get; }
    public string Comment { get; init; }

    public string UniquePath { get; }
    public string NamedPath { get; }

    public RegexElement(Enclosure[] enclosures, string regex, Palette palette = null, string comment = null)
    {
        Enclosures = enclosures;
        Regex = regex;
        Palette = palette;
        Comment = comment;

        UniquePath = string.Join('.', enclosures.Select(x => x.Ordinal));
        NamedPath =
            enclosures.Length == 0 ? string.Empty : // todo: Enclosures is sometimes empty b/c we're lazy when constructing boundaries; let's not do that
            enclosures.OfType<RootEnclosure>().Single().RootTypeName
            + (enclosures.Any(x => x is NamedEnclosure) ? "." + string.Join('.', enclosures.OfType<NamedEnclosure>().Select(x => x.Name)) : "");
    }


    public Enclosure[] PropEnclosures => Enclosures.OfType<NamedEnclosure>().ToArray();
    public IEnumerable<Enclosure> VisibleEnclosures => Enclosures.Where(e => e is not RootEnclosure);


    public override string ToString() =>
        Regex
        + (Comment == null ? "" : $" # {Comment}");
}