namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class RegexElement
{
    public Enclosure[] Enclosures { get; }
    public string Regex { get; init; }
    public string Comment { get; init; }

    public string UniquePath { get; }
    public string NamedPath { get; }

    public bool DoNotAddPrecedingSpace { get; }

    public bool SpacesGloballyDisallowedByAnyAncestor => Enclosures.Any(x => x.SpaceDisposition == SpaceDisposition.DisallowedGlobal);
    public bool SpacesDisallowedGloballyOrLocally => SpacesGloballyDisallowedByAnyAncestor || ParentEnclosure.SpaceDisposition == SpaceDisposition.DisallowedLocal;

    public IEnumerable<Enclosure> VisibleEnclosures => Enclosures.Where(e => e is not RootEnclosure);

    // The last enclosure, or if this is a group open, the second to last enclosure (since "parent" semantically should mean "container",
    // but group open elements are represented themselves as their last enclosure for display coloring purposes)
    public Enclosure ParentEnclosure =>
        this is IGroupOpen ? Enclosures.Take(Enclosures.Length - 1).LastOrDefault()
        : Enclosures.LastOrDefault();

    public Enclosure ParentNamedEnclosure =>
    this is IGroupOpen ? Enclosures.Take(Enclosures.Length - 1).OfType<NamedEnclosure>().LastOrDefault()
    : Enclosures.OfType<NamedEnclosure>().LastOrDefault();

    public RegexElement(Enclosure[] enclosures, string regex, string comment = null, bool doNotAddPrecedingSpace = false)
    {
        Enclosures = enclosures;
        Regex = regex;
        Comment = comment;
        UniquePath = string.Join('.', enclosures.Select(x => x.Ordinal));
        var namedPathParts = enclosures.OfType<RootEnclosure>().Select(x => x.RootTypeName).Concat(enclosures.OfType<NamedEnclosure>().Select(x => x.Name));
        NamedPath = string.Join('.', namedPathParts);
        var namedPathPartsRelativeToRoot = namedPathParts.Skip(1);
        DoNotAddPrecedingSpace = doNotAddPrecedingSpace;
    }

    public override string ToString() =>
        Regex
        + (Comment == null ? "" : $" # {Comment}");
}