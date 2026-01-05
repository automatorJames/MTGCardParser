namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class RegexElement
{
    public Enclosure[] Enclosures { get; }
    public string Regex { get; init; }
    public HexPalette Palette { init; get; }
    public string Comment { get; init; }

    public string UniquePath { get; }
    public string NamedPath { get; }
    public string NamedPathRelativeToRoot { get; }
    public int Depth { get; }

    public bool SpacesGloballyDisallowedByAnyAncestor => Enclosures.Any(x => x.SpaceDisposition == SpaceDisposition.DisallowedGlobal);
    public bool SpacesDisallowedGloballyOrLocally => SpacesGloballyDisallowedByAnyAncestor || ParentEnclosure.SpaceDisposition == SpaceDisposition.DisallowedLocal;

    public Enclosure[] PropEnclosures => Enclosures.OfType<NamedEnclosure>().ToArray();
    public IEnumerable<Enclosure> VisibleEnclosures => Enclosures.Where(e => e is not RootEnclosure);

    // The last enclosure, or if this is a group open, the second to last enclosure (since "parent" semantically should mean "container",
    // but group open elements are represented themselves as their last enclosure for display coloring purposes)
    public Enclosure ParentEnclosure =>
        this is IGroupOpen ? Enclosures.Take(Enclosures.Length - 1).LastOrDefault()
        : Enclosures.LastOrDefault();

    public RegexElement(Enclosure[] enclosures, string regex, HexPalette palette = null, string comment = null)
    {
        Enclosures = enclosures;
        Regex = regex;
        Palette = palette;
        Comment = comment;
        UniquePath = string.Join('.', enclosures.Select(x => x.Ordinal));
        var namedPathParts = enclosures.OfType<RootEnclosure>().Select(x => x.RootTypeName).Concat(enclosures.OfType<NamedEnclosure>().Select(x => x.Name));
        NamedPath = string.Join('.', namedPathParts);
        NamedPathRelativeToRoot = string.Join(".", namedPathParts.Skip(1));
        Depth = enclosures.Count();
    }


    public override string ToString() =>
        Regex
        + (Comment == null ? "" : $" # {Comment}");
}